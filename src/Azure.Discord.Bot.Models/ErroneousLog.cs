using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Azure.Discord.Bot.Models;

[Table("erroneous_logs")]
public class ErroneousLog
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Column("id")]
	public long Id { get; set; }

	[Column("message")]
	public required string Message { get; set; }

	[Column("logged_at")]
	public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

}
