using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class MessageDTO
{
	[JsonPropertyName("text")]
	public string? Text { get; set; }

	[JsonPropertyName("html")]
	public string? Html { get; set; }

	[JsonPropertyName("markdown")]
	public string? Markdown { get; set; }
}

