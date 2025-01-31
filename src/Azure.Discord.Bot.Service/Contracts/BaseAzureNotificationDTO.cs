using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class BaseAzureNotificationDTO<T> where T : class
{
	[JsonPropertyName("subscriptionId")]
	public required string SubscriptionId { get; set; }

	[JsonPropertyName("notificationId")]
	public int? NotificationId { get; set; }

	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("eventType")]
	public required string EventType { get; set; }

	[JsonPropertyName("publisherId")]
	public required string PublisherId { get; set; }

	[JsonPropertyName("message")]
	public MessageDTO? Message { get; set; }

	[JsonPropertyName("detailedMessage")]
	public MessageDTO? DetailedMessage { get; set; }

	[JsonPropertyName("resource")]
	public required T Resource { get; set; }

	[JsonPropertyName("resourceContainers")]
	public ResourceContainerDTO? ResourceContainers { get; set; }

	[JsonPropertyName("resourceVersion")]
	public string? ResourceVersion { get; set; }

	[JsonPropertyName("createdDate")]
	public DateTime? CreatedDate { get; set; }
}
