using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class PullRequestResourceDTO
{
	[JsonPropertyName("repository")]
	public RepositoryDTO? Repository { get; set; }

	[JsonPropertyName("pullRequestId")]
	public int PullRequestId { get; set; }

	[JsonPropertyName("codeReviewId")]
	public int? CodeReviewId { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }

	[JsonPropertyName("createdBy")]
	public object? CreatedBy { get; set; }

	[JsonPropertyName("creationDate")]
	public DateTime CreationDate { get; set; }

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

	[JsonPropertyName("isDraft")]
	public bool IsDraft { get; set; }

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

	[JsonPropertyName("supportsIterations")]
	public bool SupportsIterations { get; set; }

	[JsonPropertyName("artifactId")]
	public string? ArtifactId { get; set; }
}

