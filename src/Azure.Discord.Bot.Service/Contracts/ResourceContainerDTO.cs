using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class ResourceContainerDTO
{
	[JsonPropertyName("collection")]
	public ResourceContainerItemDTO? Collection { get; set; }

	[JsonPropertyName("account")]
	public ResourceContainerItemDTO? Account { get; set; }

	[JsonPropertyName("project")]
	public ResourceContainerItemDTO? Project { get; set; }

}
