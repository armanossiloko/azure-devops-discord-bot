namespace Azure.Discord.Bot.Service.Configuration;

public class ApplicationOptions
{
	public const string OptionsName = "Application";

	public required string DatabaseProvider { get; set; }

	/// <summary>
	/// API key necessary to access this API.
	/// </summary>
	public string? ApiKey { get; set; }

	/// <summary>
	/// Represents the token needed to access Discord's API.
	/// </summary>
	public required string DiscordToken { get; set; }

	public bool DumpConfigurationOnStartup { get; set; }

}