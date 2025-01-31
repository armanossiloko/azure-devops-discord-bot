using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class ReviewerDTO
{

	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }

	[JsonPropertyName("uniqueName")]
	public string? UniqueName { get; set; }

	[JsonPropertyName("isRequired")]
	public bool? IsRequired { get; set; }
}
