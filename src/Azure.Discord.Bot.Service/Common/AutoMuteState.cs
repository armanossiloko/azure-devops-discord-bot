using Discord.Interactions;

namespace Azure.Discord.Bot.Service.Common;

public enum AutoMuteState
{
	[ChoiceDisplay("Disable")]
	Disable,
	[ChoiceDisplay("Enable")]
	Enable,
}
