using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class CommitDTO
{
	[JsonPropertyName("commitId")]
	public required string CommitId { get; set; }

	[JsonPropertyName("url")]
	public string? Url { get; set; }
}
