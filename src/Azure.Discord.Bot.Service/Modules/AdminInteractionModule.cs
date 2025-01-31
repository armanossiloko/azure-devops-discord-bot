using Azure.Discord.Bot.DataAccess;
using Azure.Discord.Bot.Models;
using Azure.Discord.Bot.Service.Attributes;
using Azure.Discord.Bot.Service.Handlers;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Azure.Discord.Bot.Service.Modules;

public class AdminInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
	private static readonly char[] _reviewerSeparators = [',', '|', '-', ';'];
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<AdminInteractionModule> _logger;

	public AdminInteractionModule(
		IServiceProvider serviceProvider,
		ILogger<AdminInteractionModule> logger
		)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	[AuthorizeAdmin(Action = "LinkOrganization")]
	[SlashCommand("link", "Link or update an Azure DevOps organization configuration.")]
	public async Task LinkOrganizationAsync(
		[Summary(description: "User friendly name of the organization that is being added.")] string name,
		[Summary(description: "Base URL of the organization in Azure DevOps.")] string? organizationUrl = null,
		[Summary(description: "Token for Discord to use to access Azure DevOps.")] string? token = null
	)
	{
		var context = _serviceProvider.GetRequiredService<DiscordDbContext>();
		var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

		if (organizationUrl is null || token is null)
		{
			var server = await context.Servers
				.AsNoTracking()
				.Include(o => o.LinkedOrganizations)
				.FirstOrDefaultAsync(s => s.Id == Context.Guild.Id);
			if (server is null)
			{
				await RespondAsync("Server_not_found", ephemeral: true);
				return;
			}

			var linkedOrg = server
				.LinkedOrganizations
				.FirstOrDefault(o => o.DisplayName == name);
			if (linkedOrg is null)
			{
				await RespondAsync("You must provide both an Organization URL and a PAT.", ephemeral: true);
				return;
			}

			linkedOrg.OrganizationUrl ??= organizationUrl;
			linkedOrg.Token ??= token;
			context.LinkedOrganizations.Update(linkedOrg);
			await context.SaveChangesAsync();
			await RespondAsync("Organization_linked", ephemeral: true);
			return;
		}

		var newLinkedOrg = new LinkedServerOrganization()
		{
			DisplayName = name,
			ServerId = Context.Guild.Id,
			OrganizationUrl = organizationUrl.EnsureTrailing('/'),
			Token = token,
		};

		await context.LinkedOrganizations.AddAsync(newLinkedOrg);
		await context.SaveChangesAsync();
		await RespondAsync($"Organization {name} has been successfully configured.", ephemeral: true);
	}

	[AuthorizeAdmin(Action = "UnlinkOrganization")]
	[SlashCommand("unlink", "Unlink a specific Azure DevOps organization from the server.")]
	public async Task UnlinkOrganizationAsync(
		[Summary(description: "Name of the organization to retrieve item details for.")]
		[Autocomplete(typeof(LinkedOrganizationsAutocompleteHandler))]
		string? organizationName
	)
	{
		var context = _serviceProvider.GetRequiredService<DiscordDbContext>();

		var linkedOrg = await context
			.LinkedOrganizations
			.FirstOrDefaultAsync(o => o.DisplayName == organizationName);
		if (linkedOrg is null)
		{
			await RespondAsync($"Organization link with name {organizationName} could not be found.", ephemeral: true);
			return;
		}

		context.LinkedOrganizations.Remove(linkedOrg);
		await context.SaveChangesAsync();
		await RespondAsync($"Organization {organizationName} has been successfully removed.", ephemeral: true);
	}

	[AuthorizeAdmin(Action = "RegenerateApiKey")]
	[SlashCommand("regenerate-api-key", "Regenerate the API key necessary for Azure DevOps to send notifications.")]
	public async Task RegenerateApiKeyAsync()
	{
		var context = _serviceProvider.GetRequiredService<DiscordDbContext>();

		var server = await context
			.Servers
			.FirstOrDefaultAsync(o => o.Id == Context.Guild.Id);
		if (server is null)
		{
			await RespondAsync("The bot could not find your server.", ephemeral: true);
			return;
		}

		server.Key = Guid.NewGuid().ToString();
		context.Servers.Update(server);
		await context.SaveChangesAsync();
		await RespondAsync($"Server API key has been regenerated: {server.Key}.", ephemeral: true);
	}

	[AuthorizeAdmin(Action = "Subscribe")]
	[SlashCommand("subscribe", "Subscribe to an Azure DevOps web hook event.")]
	public async Task SubscribeAsync(
		[Autocomplete(typeof(AzureDevOpsEventTypesAutocompleteHandler))] string eventType,
		[ChannelTypes(ChannelType.Text)] IChannel channel,
		[Autocomplete(typeof(LinkedOrganizationsAutocompleteHandler))]
		[Summary(description: "Name of the organization (required only if the Discord server has multiple linked organizations.")]
		string? organizationName = null,
		[Autocomplete(typeof(AzureDevOpsProjectsAutocompleteHandler))]
		[Summary("project", "Name of the Azure DevOps organization project whose PRs to subscribe to.")]
		string? projectId = null,
		[Summary(description: "Name of the repository who PRs to subscribe to.")] string? repositoryName = null,
		[Summary(description: "Name of the branch the newly created PR has to target.")] string? targetBranchName = null,
		[Summary(description: "Comma-separated full names of the team (or specific users) the PR is reviewed by.")] string? reviewerNames = null
	)
	{
		var context = _serviceProvider.GetRequiredService<DiscordDbContext>();
		var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

		LinkedServerOrganization? organization;
		var organizations = await context.LinkedOrganizations.Where(x => x.ServerId == Context.Guild.Id).ToListAsync();
		if (organizations.Count == 0)
		{
			await RespondAsync($"No organizations configured; use the /link command to link an organization to this server first.", ephemeral: true);
			return;
		}
		else if (organizations.Count == 1)
		{
			organization = organizations[0];
		}
		else
		{
			if (organizationName is null)
			{
				await RespondAsync($"An organization name parameter must be provided.", ephemeral: true);
				return;
			}

			organization = organizations.SingleOrDefault(x => x.DisplayName == organizationName);
		}

		if (organization is null)
		{
			await RespondAsync($"Your organization {organizationName} has not been found.", ephemeral: true);
			return;
		}

		if (organization.OrganizationUrl is null || organization.Token is null)
		{
			await RespondAsync($"Your organization {organizationName} has no URL or PAT configured.", ephemeral: true);
			return;
		}

		var credentials = new VssBasicCredential(string.Empty, organization.Token);
		using var connection = new VssConnection(new Uri(organization.OrganizationUrl), credentials);

		var projectHttpClient = connection.GetClient<ProjectHttpClient>();
		var projects = await projectHttpClient.GetProjects();

		var filter = new SubscriptionMetadata
		{
			RepositoryName = repositoryName,
			ReviewerNames = reviewerNames?.Split(_reviewerSeparators, StringSplitOptions.RemoveEmptyEntries),
			TargetBranch = targetBranchName,
		};

		if (projectId is not null)
		{
			if (!Guid.TryParse(projectId, out var projectGuid))
			{
				await RespondAsync($"The project ID '{projectId}' is not valid.", ephemeral: true);
				return;
			}

			var selectedProject = projects.FirstOrDefault(x => x.Id == projectGuid);
			if (selectedProject is null)
			{
				await RespondAsync($"The project {projectId} does not exist within the selected organization.", ephemeral: true);
				return;
			}

			filter.ProjectId = projectId;
		}
		
		var subscription = new Subscription
		{
			Id = Guid.NewGuid().ToString(),
			EventType = eventType,
			ChannelId = channel.Id,
			Filter = filter,
			OrganizationId = organization.Id,
		};

		await context.Subscriptions.AddAsync(subscription);
		await context.SaveChangesAsync();

		await RespondAsync($"Event notifications for `{eventType}` will be sent to `{channel.Name}`.", ephemeral: true);
	}

}
