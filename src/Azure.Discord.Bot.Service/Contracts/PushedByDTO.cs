using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class PushedByDTO
{
	[JsonPropertyName("displayName")]
	public string DisplayName { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; }

	[JsonPropertyName("_links")]
	public LinksContainerDTO Links { get; set; }

	[JsonPropertyName("id")]
	public string Id { get; set; }

	[JsonPropertyName("uniqueName")]
	public string UniqueName { get; set; }

	[JsonPropertyName("imageUrl")]
	public string? ImageUrl { get; set; }

	[JsonPropertyName("descriptor")]
	public string Descriptor { get; set; }
}