using Azure.Discord.Bot.DataAccess;
using Azure.Discord.Bot.Service.Configuration;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System.Reflection;
using IResult = Discord.Interactions.IResult;

namespace Azure.Discord.Bot.Service.Workers;

public class DiscordWorker : BackgroundService
{
	private readonly ILogger<DiscordWorker> _logger;
	private readonly IOptions<ApplicationOptions> _applicationOptions;
	private readonly IConfiguration _configuration;
	private readonly DiscordSocketClient _client;
	private readonly InteractionService _interactionService;
	private readonly IServiceProvider _serviceProvider;

	public DiscordWorker(
		ILogger<DiscordWorker> logger,
		IOptions<ApplicationOptions> applicationOptions,
		IConfiguration configuration,
		DiscordSocketClient client,
		InteractionService interactionService,
		IServiceProvider serviceProvider
		)
	{
		_logger = logger;
		_applicationOptions = applicationOptions;
		_configuration = configuration;
		_client = client;
		_interactionService = interactionService;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		_client.Log += OnClientLogAsync;
		_client.Ready += OnClientReadyAsync;
		_client.InteractionCreated += HandleInteractionAsync;
		_client.GuildAvailable += OnGuildAvailableAsync;

		_interactionService.Log += OnInteractionServiceLogAsync;
		_interactionService.InteractionExecuted += OnInteractionExecutedAsync;
		await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

		await _client.LoginAsync(TokenType.Bot, _applicationOptions.Value.DiscordToken);
		await _client.StartAsync();

		await Task.Delay(-1, cancellationToken);
	}

	private async Task OnGuildAvailableAsync(SocketGuild guild)
	{
		try
		{
			await using var scope = _serviceProvider.CreateAsyncScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<DiscordDbContext>();

			var server = await dbContext!.Servers.FindAsync(guild.Id);
			if (server is null)
			{
				server = new()
				{
					Id = guild.Id,
					Alias = guild.Name,
					Key = Guid.NewGuid().ToString(),
				};
				await dbContext.Servers.AddAsync(server);
				await dbContext.SaveChangesAsync();
			}
			else
			{
				if (server.Alias != guild.Name)
				{
					server.Alias = guild.Name;
					dbContext.Servers.Update(server);
					await dbContext.SaveChangesAsync();
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			throw;
		}
	}

	private Task OnClientLogAsync(LogMessage arg) => LogAsync(arg);

	private async Task OnClientReadyAsync()
	{
		try
		{
			await _interactionService.RegisterCommandsGloballyAsync();
			_logger.LogInformation("InteractionService has been connected.");
		}
		catch (ApplicationCommandException ex)
		{
			_logger.LogError(ex, "An exception occurred during setup of commands with errors {@Errors}", ex.Errors);
		}
		catch (HttpException ex)
		{
			_logger.LogError(ex, "An exception occurred during setup of commands with errors {@Errors}", ex.Errors);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An exception occurred during setup of commands with errors {@Errors}", ex.Message);
		}
	}

	/// <inheritdoc cref="BaseSocketClient.InteractionCreated"/>
	private async Task HandleInteractionAsync(SocketInteraction interaction)
	{
		try
		{
			// Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
			var context = new SocketInteractionContext(_client, interaction);

			// Execute the incoming command.
			var result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

			// Due to async nature of InteractionFramework, the result here may always be success.
			// That's why we also need to handle the InteractionExecuted event.
			if (!result.IsSuccess)
			{
				switch (result.Error)
				{
					case InteractionCommandError.UnmetPrecondition:
						// TODO: Implement
						break;
					default:
						break;
				}
			}
		}
		catch
		{
			// If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
			// response, or at least let the user know that something went wrong during the command execution.
			if (interaction.Type is InteractionType.ApplicationCommand)
			{
				await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
			}
		}
	}

	private Task OnInteractionServiceLogAsync(LogMessage arg) => LogAsync(arg);

	private Task OnInteractionExecutedAsync(ICommandInfo commandInfo, IInteractionContext context, IResult result)
	{
		if (!result.IsSuccess)
		{
			_logger.LogError("Handling interaction failed with error {Error} (reason: {ErrorReason}).", result.Error, result.ErrorReason);
		}
		return Task.CompletedTask;
	}

	private Task LogAsync(LogMessage log)
	{
		var logLevel = log.Severity.ToLogLevel();
		if (_logger.IsEnabled(logLevel))
		{
			if (log.Exception is not null)
			{
				_logger.Log(logLevel, log.Exception, "An internal exception from {Source} has occurred. {Message}", log.Source, log.Message);
			}
			else
			{
				_logger.Log(logLevel, "A message from {Source} is being propagated: '{Message}'.", log.Source, log.Message);
			}
		}
		return Task.CompletedTask;
	}

}