using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class ResourceContainerItemDTO
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("baseUrl")]
	public string? BaseUrl { get; set; }

}