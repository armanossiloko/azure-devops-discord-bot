using Azure.Discord.Bot.DataAccess;
using Azure.Discord.Bot.Service.Attributes;
using Azure.Discord.Bot.Service.Contracts;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Azure.Discord.Bot.Service.Controllers;

[ApiController]
[Route("api/notifications")]
[AzureHook]
public class AzureNotificationsController : ControllerBase
{
	private readonly DiscordDbContext _context;
	private readonly DiscordSocketClient _client;

	public AzureNotificationsController(
		DiscordDbContext context,
		DiscordSocketClient client
		)
	{
		_context = context;
		_client = client;
	}

	[HttpPost("pulls/created")]
	[HttpPost("pulls/updated")]
	[HttpPost("pulls/merged")]
	public async Task<IActionResult> OnPullRequestCreatedOrUpdated([FromBody] BaseAzureNotificationDTO<ResourceDTO> data)
	{
		var orgUrl = data.ResourceContainers?.Account?.BaseUrl;
		if (string.IsNullOrEmpty(orgUrl))
		{
			// For unsuccessful status codes, Azure auto-disables the web hook after N amount of time.
			return Ok();
		}

		if (data.EventType != "git.pullrequest.created"
			&& data.EventType != "git.pullrequest.updated"
			&& data.EventType != "git.pullrequest.merged"
			)
		{
			return BadRequest();
		}

		var subscriptions = await _context.Subscriptions
			.Include(s => s.Organization)
			.Include(s => s.Filter)
			.Where(s => s.Organization!.OrganizationUrl == orgUrl && s.EventType == data.EventType)
			.ToListAsync();
		if (subscriptions.Count == 0)
		{
			// For unsuccessful status codes, Azure auto-disables the web hook after N amount of time.
			return Ok();
		}

		var projectId = data.ResourceContainers?.Project?.Id;
		var repositoryName = data.Resource.Repository?.Name;
		var targetBranch = data.Resource.TargetRefName;
		var reviewerNames = data.Resource.Reviewers?.Select(x => x.DisplayName).ToList() ?? [];

		var subscriptionChannels = new List<ulong>();
		foreach (var subscription in subscriptions)
		{
			if (subscription.Filter is not null)
			{
				if (subscription.Filter!.ProjectId is not null)
				{
					if (!subscription.Filter!.ProjectId.Equals(projectId))
					{
						continue;
					}
				}

				if (subscription.Filter.RepositoryName is not null)
				{
					if (!subscription.Filter.RepositoryName.Equals(repositoryName))
					{
						continue;
					}
				}

				if (subscription.Filter.TargetBranch is not null)
				{
					if (!subscription.Filter.TargetBranch.Equals(targetBranch))
					{
						continue;
					}
				}

				if (subscription.Filter.ReviewerNames is not null)
				{
					if (!subscription.Filter.ReviewerNames.Any(reviewerNames.Contains))
					{
						continue;
					}
				}
			}

			subscriptionChannels.Add(subscription.ChannelId);
		}

		if (subscriptionChannels.Count == 0)
		{
			return NoContent();
		}

		var embedBuilder = new EmbedBuilder()
			.WithDescription(data.Message?.Markdown ?? "")
			.AddField("Source", $"`{data.Resource.SourceRefName}`", true)
			.AddField("Target", $"`{data.Resource.TargetRefName}`", true)
			.AddField("Status", $"`{data.Resource.Status}`", true)
			.AddField("Merge status", $"`{data.Resource.MergeStatus}`");

		if (data.Resource.Description is not null)
		{
			embedBuilder = embedBuilder.AddField("Description", data.Resource.Description);
		}

		if (data.Resource.Reviewers is not null && data.Resource.Reviewers.Count > 0)
		{
			var reviewers = new StringBuilder();
			foreach (var item in data.Resource.Reviewers ?? [])
			{
				reviewers.AppendLine(item.DisplayName);
			}
			embedBuilder = embedBuilder.AddField("Reviewers", reviewers.ToString());
		}
		var embed = embedBuilder.Build();

		foreach (var subscriptionChannelId in subscriptionChannels.Distinct())
		{
			if (_client.GetChannel(subscriptionChannelId) is IMessageChannel channel)
			{
				await channel.SendMessageAsync(embed: embed);
			}
		}

		return Ok();
	}

	[HttpPost("pulls/commented")]
	public async Task<IActionResult> OnPullRequestCommented([FromBody] BaseAzureNotificationDTO<ResourceDTO> data)
	{
		var orgUrl = data.ResourceContainers?.Account?.BaseUrl;
		if (string.IsNullOrEmpty(orgUrl))
		{
			// For unsuccessful status codes, Azure auto-disables the web hook after N amount of time.
			return Ok();
		}

		if (data.EventType != "ms.vss-code.git-pullrequest-comment-event")
		{
			return BadRequest();
		}

		var subscriptions = await _context.Subscriptions
			.Include(s => s.Organization)
			.Include(s => s.Filter)
			.Where(s => s.Organization!.OrganizationUrl == orgUrl && s.EventType == data.EventType)
			.ToListAsync();
		if (subscriptions.Count == 0)
		{
			// For unsuccessful status codes, Azure auto-disables the web hook after N amount of time.
			return Ok();
		}

		var projectId = data.ResourceContainers?.Project?.Id;
		var repositoryName = data.Resource.PullRequest?.Repository?.Name;
		var targetBranch = data.Resource.PullRequest?.TargetRefName;
		var reviewerNames = data.Resource.PullRequest?.Reviewers?.Select(x => x.DisplayName).ToList() ?? [];

		var subscriptionChannels = new List<ulong>();
		foreach (var subscription in subscriptions)
		{
			if (subscription.Filter is not null)
			{
				if (subscription.Filter!.ProjectId is not null)
				{
					if (!subscription.Filter!.ProjectId.Equals(projectId))
					{
						continue;
					}
				}

				if (subscription.Filter.RepositoryName is not null)
				{
					if (!subscription.Filter.RepositoryName.Equals(repositoryName))
					{
						continue;
					}
				}

				if (subscription.Filter.TargetBranch is not null)
				{
					if (!subscription.Filter.TargetBranch.Equals(targetBranch))
					{
						continue;
					}
				}

				if (subscription.Filter.ReviewerNames is not null)
				{
					if (!subscription.Filter.ReviewerNames.Any(reviewerNames.Contains))
					{
						continue;
					}
				}
			}

			subscriptionChannels.Add(subscription.ChannelId);
		}

		if (subscriptionChannels.Count == 0)
		{
			return NoContent();
		}

		var embedBuilder = new EmbedBuilder()
			.WithDescription(data.Message?.Markdown ?? "")
			.AddField("Source", $"`{data.Resource.PullRequest?.SourceRefName}`", true)
			.AddField("Target", $"`{data.Resource.PullRequest?.TargetRefName}`", true)
			.AddField("Status", $"`{data.Resource.PullRequest?.Status}`")
			.AddField("Merge status", $"`{data.Resource.PullRequest?.MergeStatus}`", true);

		if (data.Resource.Reviewers is not null && data.Resource.Reviewers.Count > 0)
		{
			var reviewers = new StringBuilder();
			foreach (var item in data.Resource.Reviewers ?? [])
			{
				reviewers.AppendLine(item.DisplayName);
			}
			embedBuilder = embedBuilder.AddField("Reviewers", reviewers.ToString());
		}
		var embed = embedBuilder.Build();

		foreach (var subscriptionChannelId in subscriptionChannels.Distinct())
		{
			if (_client.GetChannel(subscriptionChannelId) is IMessageChannel channel)
			{
				await channel.SendMessageAsync(embed: embed);
			}
		}

		return Ok();
	}

	[HttpPost("code/pushed")]
	public async Task<IActionResult> OnCodePushed([FromBody] BaseAzureNotificationDTO<ResourceDTO> data)
	{
		var orgUrl = data.ResourceContainers?.Account?.BaseUrl;
		if (string.IsNullOrEmpty(orgUrl))
		{
			// For unsuccessful status codes, Azure auto-disables the web hook after N amount of time.
			return Ok();
		}

		if (data.EventType != "git.push")
		{
			return BadRequest();
		}

		var subscriptions = await _context.Subscriptions
			.Include(s => s.Organization)
			.Include(s => s.Filter)
			.Where(s => s.Organization!.OrganizationUrl == orgUrl && s.EventType == data.EventType)
			.ToListAsync();

		subscriptions.Add(new()
		{
			Id = data.Id,
			EventType = data.EventType,
		});

		if (subscriptions.Count == 0)
		{
			// For unsuccessful status codes, Azure auto-disables the web hook after N amount of time.
			return Ok();
		}

		var projectId = data.ResourceContainers?.Project?.Id;
		var repositoryName = data.Resource.Repository?.Name;
		var targetBranch = data.Resource.TargetRefName;
		var reviewerNames = data.Resource.Reviewers?.Select(x => x.DisplayName).ToList() ?? [];

		var subscriptionChannels = new List<ulong>();
		foreach (var subscription in subscriptions)
		{
			if (subscription.Filter is not null)
			{
				if (subscription.Filter!.ProjectId is not null)
				{
					if (!subscription.Filter!.ProjectId.Equals(projectId))
					{
						continue;
					}
				}

				if (subscription.Filter.RepositoryName is not null)
				{
					if (!subscription.Filter.RepositoryName.Equals(repositoryName))
					{
						continue;
					}
				}
			}

			subscriptionChannels.Add(subscription.ChannelId);
		}

		if (subscriptionChannels.Count == 0)
		{
			return NoContent();
		}

		var embed = new EmbedBuilder()
			.WithDescription(data.Message?.Markdown ?? "")
			.AddField("Link", $"{data.Resource.Url}")
			.Build();

		foreach (var subscriptionChannelId in subscriptionChannels.Distinct())
		{
			if (_client.GetChannel(subscriptionChannelId) is IMessageChannel channel)
			{
				await channel.SendMessageAsync(embed: embed);
			}
		}

		return Ok();
	}

}
