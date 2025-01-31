using Azure.Discord.Bot.Service;
using Azure.Discord.Bot.Service.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Azure.Discord.Bot.Integration.Tests;

public class DiscordFactory : WebApplicationFactory<IProgram>, IAsyncLifetime
{
	/// <remarks>
	///     The scope of a <see cref="_dbContainer"/> is per test class, not per test method!
	/// </remarks>
	private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
		.WithUsername("discord")
		.WithPassword("discord")
		.WithDatabase("discorddb")
		.Build();
	private readonly IMessageSink _messageSink;

	public DiscordFactory(IMessageSink messageSink)
	{
		_messageSink = messageSink;
	}

	protected override IWebHostBuilder? CreateWebHostBuilder()
	{
		var builder = base.CreateWebHostBuilder();
		//builder.UseEnvironment();
		return builder;
	}

	protected override IHost CreateHost(IHostBuilder builder)
	{
		builder.UseSerilog((_, loggerConfiguration) =>
		{
			loggerConfiguration.WriteTo.Console();
			loggerConfiguration.WriteTo.TestOutput(_messageSink);
		});

		return base.CreateHost(builder);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureLogging(logging =>
		{
			logging.ClearProviders();
			logging.SetMinimumLevel(LogLevel.Trace);
			logging.AddConsole();
			//logging.AddXUnit(_output);
		});

		builder.ConfigureAppConfiguration((ctx, builder) =>
		{
			builder.Sources.Clear();

			// Find why .NET prioritizes this fucking dictionary over everything
			// Also find out why appsettings.ENV.json is not loaded at all even though the environment is set correctly.
			var defaultConfiguration = new Dictionary<string, string?>()
			{
				["ConnectionStrings:Npgsql"] = _dbContainer.GetConnectionString(),
				[$"{ApplicationOptions.OptionsName}:{nameof(ApplicationOptions.ApiKey)}"] = "",
				[$"{ApplicationOptions.OptionsName}:{nameof(ApplicationOptions.DiscordToken)}"] = "",
				[$"{ApplicationOptions.OptionsName}:{nameof(ApplicationOptions.DatabaseProvider)}"] = "PostgreSQL",
			};
			builder.AddInMemoryCollection(defaultConfiguration);

			// Tests can be ran from the CLI using one of the following:
			// - dotnet test -e ASPNETCORE_ENVIRONMENT=Test
			// - dotnet test --environment ASPNETCORE_ENVIRONMENT=Test
			// However, for the time being (due to Testcontainers limitations), only PostgreSQL is supported
			builder.SetBasePath(Directory.GetCurrentDirectory());
			builder//.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
				.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);
			builder.AddEnvironmentVariables();
		});

		builder.ConfigureTestServices(services =>
		{
			// Remove existing DI registrations

			// Add test DI registrations
		});
	}

	public async Task InitializeAsync()
	{
		await _dbContainer.StartAsync();
	}

	async Task IAsyncLifetime.DisposeAsync()
	{
		await _dbContainer.StopAsync();
	}

}
