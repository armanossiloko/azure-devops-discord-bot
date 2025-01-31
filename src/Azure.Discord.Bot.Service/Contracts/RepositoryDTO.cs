using System.Text.Json.Serialization;

namespace Azure.Discord.Bot.Service.Contracts;

public class RepositoryDTO
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("url")]
	public string? Url { get; set; }

	[JsonPropertyName("project")]
	public ProjectDTO? Project { get; set; }

	[JsonPropertyName("defaultBranch")]
	public string? DefaultBranch { get; set; }

	[JsonPropertyName("remoteUrl")]
	public string? RemoteUrl { get; set; }

	[JsonPropertyName("sshUrl")]
	public string? SshUrl { get; set; }

	[JsonPropertyName("webUrl")]
	public string? WebUrl { get; set; }

	[JsonPropertyName("isDisabled")]
	public bool? IsDisabled { get; set; }

	[JsonPropertyName("isInMaintenance")]
	public bool? IsInMaintenance { get; set; }

}

