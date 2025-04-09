using Azure.Discord.Bot.DataAccess;
using Azure.Discord.Bot.Models;
using Azure.Discord.Bot.Service.Handlers;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Azure.Discord.Bot.Service.Modules;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
	private static readonly char[] _separators = [',', '|', ' ', '-', ';'];
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<InteractionModule> _logger;

	public InteractionModule(
		IServiceProvider serviceProvider,
		ILogger<InteractionModule> logger
		)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	[SlashCommand("tfs", "Send Azure DevOps items (epic, feature, bug, etc.) with their details to the channel.")]
	public Task GetBacklogItemsDetailsAsync(
		[Summary(description: "Comma-separated IDs of the backlog items to provide details for.")]
		string items,
		[Summary(description: "Name of the organization to retrieve item details for.")]
		[Autocomplete(typeof(LinkedOrganizationsAutocompleteHandler))]
		string? organizationName = null
	)
	{
		return DisplayItemsDetailsAsync(items, organizationName);
	}

	[SlashCommand("display", "Send Azure DevOps items (epic, feature, bug, etc.) with their details to the channel.")]
	public async Task DisplayItemsDetailsAsync(
		[Summary(description: "Comma-separated IDs of the backlog items to provide details for.")]
		string items,
		[Summary(description: "Name of the organization to retrieve item details for.")]
		[Autocomplete(typeof(LinkedOrganizationsAutocompleteHandler))]
		string? organizationName = null
	)
	{
		try
		{
			var context = _serviceProvider.GetRequiredService<DiscordDbContext>();
			var linkedOrgs = await context.LinkedOrganizations
				.AsNoTracking()
				.Where(o => o.ServerId == Context.Guild.Id)
				.ToListAsync();

			LinkedServerOrganization? linkedOrg;

			if (linkedOrgs.Count == 0)
			{
				await RespondAsync("No Azure DevOps organizations are linked for this server.", ephemeral: true);
				return;
			}
			else if (linkedOrgs.Count == 1)
			{
				linkedOrg = linkedOrgs[0];
			}
			else
			{
				if (organizationName is null)
				{
					await RespondAsync("Multiple organizations linked to this server; you must provide an organization name to use this command.", ephemeral: true);
					return;
				}

				linkedOrg = linkedOrgs.FirstOrDefault(o => o.DisplayName == organizationName);
			}

			if (linkedOrg is null)
			{
				await RespondAsync($"No Azure DevOps organizations are linked with the Name = {organizationName}.", ephemeral: true);
				return;
			}

			if (linkedOrg.OrganizationUrl is null || linkedOrg.Token is null)
			{
				await RespondAsync($"The linked Azure DevOps organization is not properly configured. Make sure to configure both the URL and PAT.", ephemeral: true);
				return;
			}

			await DeferAsync();

			List<int> itemIds = [];
			foreach (var item in items.Split(_separators, StringSplitOptions.RemoveEmptyEntries))
			{
				if (!int.TryParse(item, out var parsed))
				{
					await RespondAsync($"Item '{item}' could not be parsed.", ephemeral: true);
					continue;
				}

				itemIds.Add(parsed);
			}

			if (itemIds.Count == 0)
			{
				await RespondAsync("You must provide at least one item ID.", ephemeral: true);
				return;
			}

			var credentials = new VssBasicCredential(string.Empty, linkedOrg.Token);
			using var connection = new VssConnection(new Uri(linkedOrg.OrganizationUrl), credentials);
			var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();
			List<WorkItem> workItems = [];

			try
			{
				workItems = await workItemTrackingClient.GetWorkItemsAsync(itemIds, expand: WorkItemExpand.All);
			}
			catch (VssServiceException ex)
			{
				await RespondAsync(ex.Message, ephemeral: true);
				return;
			}

			foreach (var workItem in workItems)
			{
				workItem.Links.Links.TryGetValue("html", out var html);
				if (html is not ReferenceLink link)
				{
					await FollowupAsync($"Azure DevOps did not provide an URL for backlog item with ID {workItem.Id}.");
					return;
				}

				var type = workItem.Fields["System.WorkItemType"].ToString();
				var title = workItem.Fields["System.Title"].ToString();
				var state = workItem.Fields["System.State"].ToString();
				_ = workItem.Fields.TryGetValue("System.AssignedTo", out var assignedToValue);
				var assignedTo = assignedToValue as IdentityRef;

				var builder = new EmbedBuilder();
				var color = type switch
				{
					"Epic" => Color.Orange,
					"Feature" => Color.Purple,
					"Bug" => Color.Red,
					"Product Backlog Item" or "User Story" => Color.Blue,
					"Task" => Color.LightOrange,
					_ => Color.Default,
				};

				builder.Title = title;
				builder.Url = link.Href;
				builder.Color = color;


				builder.AddField("Type", type, true);
				builder.AddField("State", state, true);
				builder.AddField("Assigned to", assignedTo?.DisplayName ?? "Unassigned", true);


				if (workItem.Fields.TryGetValue("System.AreaPath", out var areaPath))
				{
					builder.AddField("Area", areaPath.ToString(), true);
				}

				if (workItem.Fields.TryGetValue("System.IterationPath", out var iterationPath))
				{
					builder.AddField("Iteration", iterationPath.ToString(), true);
				}

				if (workItem.Fields.TryGetValue("Microsoft.VSTS.Common.Severity", out var severity))
				{
					builder.AddField("Severity", severity.ToString(), true);
				}

				if (workItem.Fields.TryGetValue("System.Reason", out var reason))
				{
					builder.AddField("Reason", reason.ToString(), true);
				}

				var embed = builder.Build();
				await FollowupAsync(embed: embed);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"An exception occurred during retrieval of items {Items} for organization {OrganizationName}. {ExceptionMessage}",
				items,
				organizationName,
				ex.Message
				);
			return;
		}
	}

}
