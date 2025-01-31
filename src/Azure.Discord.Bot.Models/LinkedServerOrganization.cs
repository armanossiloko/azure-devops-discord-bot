using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.Discord.Bot.Models;

[Table("linked_server_organizations")]
public class LinkedServerOrganization
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Column("id")]
	public long Id { get; set; }

	[Column("display_name")]
	public required string DisplayName { get; set; }

	[Column("organization_url")]
	public string? OrganizationUrl { get; set; }

	[Column("token")]
	public string? Token { get; set; }

	[Column("server_id")]
	public ulong ServerId { get; set; }
	public virtual Server? Server { get; set; }

	public virtual List<Subscription> Subscriptions { get; set; } = [];

}
