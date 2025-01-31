using Azure.Discord.Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Azure.Discord.Bot.DataAccess;

public class DiscordDbContext(DbContextOptions<DiscordDbContext> options) : DbContext(options)
{
	public DbSet<Server> Servers { get; set; }
    public DbSet<LinkedServerOrganization> LinkedOrganizations { get; set; }
	public DbSet<Subscription> Subscriptions { get; set; }
	//public DbSet<ErroneousLog> ErroneousLogs { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Subscription>().OwnsOne(p => p.Filter);

	}
}
