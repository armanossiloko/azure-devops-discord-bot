using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.Discord.Bot.Models;

[Table("servers")]
public class Server
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Column("id")]
	public ulong Id { get; set; }

	[Column("alias")]
	public required string Alias { get; set; }

	[Column("key")]
	public required string Key { get; set; }

	public List<LinkedServerOrganization> LinkedOrganizations { get; set; } = [];

}
