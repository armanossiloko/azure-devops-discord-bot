using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.Discord.Bot.Models;

public class SubscriptionMetadata
{
	[Column("project_id")]
	public string? ProjectId { get; set; }

	[Column("target_branch")]
	public string? TargetBranch { get; set; }

	[Column("repository_name")]
	public string? RepositoryName { get; set; }

	[Column("reviewer_names")]
	public string[]? ReviewerNames { get; set; }

}
