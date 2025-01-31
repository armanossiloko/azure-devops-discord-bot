using Discord.Interactions;
using Discord.WebSocket;

namespace Azure.Discord.Bot.Service.Attributes;

public class AuthorizeAdminAttribute : PreconditionAttribute
{
	public required string Action { get; set; }

	public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider serviceProvider)
	{
		var client = serviceProvider.GetRequiredService<DiscordSocketClient>();

		if (context.Guild.OwnerId == context.User.Id)
		{
			return Task.FromResult(PreconditionResult.FromSuccess());
		}

		if (context.User is not SocketGuildUser user)
		{
			return Task.FromResult(PreconditionResult.FromError("User permissions could not be determined."));
		}

		var isAdmin = user.Roles.Any(r => r.Permissions.Administrator);

		return Task.FromResult(isAdmin ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"You must be an Admin to perform {Action}."));
	}

}