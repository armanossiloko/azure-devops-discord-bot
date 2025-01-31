global using Discord;
using Azure.Discord.Bot.DataAccess;
using Azure.Discord.Bot.Service.Configuration;
using Azure.Discord.Bot.Service.Handlers;
using Azure.Discord.Bot.Service.Middleware;
using Azure.Discord.Bot.Service.Workers;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

const string appName = "Discord Worker Service";
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateLogger();

try
{
	Log.Information("{AppName} starting up...", appName);

	DiscordSocketConfig socketConfig = new()
	{
		GatewayIntents = GatewayIntents.All,
		//GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
		AlwaysDownloadUsers = true,
		UseInteractionSnowflakeDate = false,
	};
	DiscordSocketClient client = new(socketConfig);

	var builder = WebApplication.CreateBuilder(args);

	Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
		.ReadFrom.Configuration(builder.Configuration)
		.Enrich.FromLogContext()
		.CreateLogger();
	builder.Logging.ClearProviders().AddSerilog();

	builder.Services.AddControllers().AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.WriteIndented = true;
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});
	builder.Services.AddSingleton<IFileSystem, FileSystem>();

	if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows)
	{
		builder.Services.AddWindowsService(options =>
		{
			options.ServiceName = "Azure.Discord.Bot.Service";
		});
	}
	else if (Environment.OSVersion.Platform == PlatformID.Unix)
	{
		builder.Services.AddSystemd();
	}
	else
	{
		throw new InvalidOperationException($"Attempted to run the application as a hosted service for OS {Environment.OSVersion.Platform}.");
	}


	var provider = builder.Configuration["Application:DatabaseProvider"]?.ToString();
	builder.Services.AddDbContext<DiscordDbContext>(options =>
	{
		switch (provider ?? "")
		{
			case "Npgsql":
				options.UseNpgsql(
					builder.Configuration.GetConnectionString("Npgsql"),
					optionsBuilder => optionsBuilder.MigrationsAssembly("Azure.Discord.Bot.DataAccess.Npgsql")
					);
				break;
			case "SQLite":
				options.UseSqlite(
					builder.Configuration.GetConnectionString("SQLite"),
					optionsBuilder => optionsBuilder.MigrationsAssembly("Azure.Discord.Bot.DataAccess.Sqlite")
					);
				break;
			default:
				throw new ArgumentOutOfRangeException($"Invalid DatabaseProvider configured ({provider}).");
		}
	});

	builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(ApplicationOptions.OptionsName));
	builder.Services.AddHostedService<DiscordWorker>();
	builder.Services.AddSingleton(socketConfig);
	builder.Services.AddSingleton(client);
	builder.Services.AddSingleton(sp => new InteractionService(sp.GetRequiredService<DiscordSocketClient>(), null));
	builder.Services.AddTransient<AzureDevOpsProjectsAutocompleteHandler>();
	builder.Services.AddTransient<AzureDevOpsEventTypesAutocompleteHandler>();
	builder.Services.AddTransient<LinkedOrganizationsAutocompleteHandler>();

	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen(options =>
	{
		var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		var xmlFile = Path.Combine(AppContext.BaseDirectory, xmlFileName);
		options.IncludeXmlComments(xmlFile);
		options.EnableAnnotations();
		options.UseOneOfForPolymorphism();
	});

	var app = builder.Build();
	var appOptions = app.Services.GetRequiredService<IOptions<ApplicationOptions>>().Value;

	using (var scope = app.Services.CreateScope())
	{
		// Run EF Core migrations
		using (var ctx = scope.ServiceProvider.GetRequiredService<DiscordDbContext>())
		{
			ctx.Database.Migrate();
		}

		// Dump current application configuration into a file
		if (appOptions.DumpConfigurationOnStartup && app.Configuration is IConfigurationRoot configurationRoot)
		{
			const string configurationDumpFile = "config.dump.txt";

			var fileSystem = scope.ServiceProvider.GetRequiredService<IFileSystem>();
			var configurationDumpPath = fileSystem.Path.Combine(Environment.CurrentDirectory, configurationDumpFile);

			if (fileSystem.File.Exists(configurationDumpPath))
			{
				fileSystem.File.Delete(configurationDumpPath);
			}

			var dump = configurationRoot.GetDebugView();
			using var streamWriter = fileSystem.File.CreateText(configurationDumpPath);
			streamWriter.Write(dump);
		}
	}

	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseMiddleware<ApiKeyProtectionMiddleware>();
	app.UseAuthentication();
	app.UseAuthorization();
	app.MapControllers();
	app.Run();

}
catch (Exception ex)
{
	Log.Fatal(ex, "{AppName} shutting down unexpectedly... {Exception}", appName, ex.Message);

	if (ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
	{
		// Thrown when adding and executing EntityFrameworkCore migrations
		throw;
	}

	return -1;
}
finally
{
	Log.Information("{AppName} shutting down...", appName);
	Log.CloseAndFlush();
}

return 0;
