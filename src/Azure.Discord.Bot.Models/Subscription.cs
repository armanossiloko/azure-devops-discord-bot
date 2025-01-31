using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.Discord.Bot.Models;

[Table("subscriptions")]
public class Subscription
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Column("id")]
	public required string Id { get; set; }

	[Column("channel_id")]
	public ulong ChannelId { get; set; }

	[Column("event_type")]
	public required string EventType { get; set; }

	[Column("organization_id")]
	public long OrganizationId { get; set; }
	public virtual LinkedServerOrganization? Organization { get; set; }

	public virtual SubscriptionMetadata? Filter { get; set; }

}
