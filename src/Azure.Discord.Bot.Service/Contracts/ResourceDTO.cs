using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class ResourceDTO
{
	[JsonPropertyName("repository")]
	public RepositoryDTO? Repository { get; set; }

	[JsonPropertyName("pullRequest")]
	public PullRequestResourceDTO? PullRequest { get; set; }

	[JsonPropertyName("pullRequestId")]
	public int? PullRequestId { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }

	[JsonPropertyName("creationDate")]
	public DateTime? CreationDate { get; set; }

	[JsonPropertyName("title")]
	public string? Title { get; set; }

	[JsonPropertyName("description")]
	public string? Description { get; set; }

	[JsonPropertyName("sourceRefName")]
	public string? SourceRefName { get; set; }

	[JsonPropertyName("targetRefName")]
	public string? TargetRefName { get; set; }

	[JsonPropertyName("mergeStatus")]
	public string? MergeStatus { get; set; }

	[JsonPropertyName("mergeId")]
	public string? MergeId { get; set; }

	[JsonPropertyName("lastMergeSourceCommit")]
	public CommitDTO? LastMergeSourceCommit { get; set; }

	[JsonPropertyName("lastMergeTargetCommit")]
	public CommitDTO? LastMergeTargetCommit { get; set; }

	[JsonPropertyName("lastMergeCommit")]
	public CommitDTO? LastMergeCommit { get; set; }

	[JsonPropertyName("reviewers")]
	public List<ReviewerDTO>? Reviewers { get; set; }

	[JsonPropertyName("url")]
	public string? Url { get; set; }

	[JsonPropertyName("_links")]
	public LinksContainerDTO? Links { get; set; }

	[JsonPropertyName("pushedBy")]
	public PushedByDTO? PushedBy { get; set; }

	[JsonPropertyName("pushId")]
	public int? PushId { get; set; }


}
