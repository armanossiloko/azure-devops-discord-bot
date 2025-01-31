using System.Net;
using System.Net.Http.Json;
using Xunit.Categories;

namespace Azure.Discord.Bot.Integration.Tests.Controllers;

[IntegrationTest]
public class AzureNotificationsControllerTests : IClassFixture<DiscordFactory>
{
	private const string _baseUrl = "api/notifications";
	private readonly DiscordFactory _discordFactory;

	public AzureNotificationsControllerTests(
		DiscordFactory discordFactory
		)
	{
		_discordFactory = discordFactory;
	}

	[Fact(Skip = "Needs to be implemented first.")]
	public async Task Test1()
	{
		// Arrange
		using var client = _discordFactory.CreateClient();

		// Act
		using var postResponse = await client.PostAsJsonAsync(_baseUrl, "");
		using var getResponse = await client.GetAsync(_baseUrl);

		// Assert
		Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
		Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
	}

}
