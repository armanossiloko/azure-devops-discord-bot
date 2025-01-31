using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class LinksContainerDTO
{
	[JsonPropertyName("avatar")]
	public LinkDTO? Avatar { get; set; }

	[JsonPropertyName("web")]
	public LinkDTO? Web { get; set; }

	[JsonPropertyName("statuses")]
	public LinkDTO? Statuses { get; set; }
}
