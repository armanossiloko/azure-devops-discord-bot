using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class LinkDTO
{
	[JsonPropertyName("href")]
	public string? Href { get; set; }
}
