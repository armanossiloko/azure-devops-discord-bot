using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class ProjectDTO
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("url")]
	public string? Url { get; set; }

	[JsonPropertyName("state")]
	public string? State { get; set; }

	[JsonPropertyName("visibility")]
	public string? Visibility { get; set; }

	[JsonPropertyName("lastUpdateTime")]
	public DateTime? LastUpdateTime { get; set; }
}

