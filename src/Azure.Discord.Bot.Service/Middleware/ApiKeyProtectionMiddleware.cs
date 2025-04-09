using Azure.Discord.Bot.DataAccess;
using Azure.Discord.Bot.Service.Attributes;
using Azure.Discord.Bot.Service.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Azure.Discord.Bot.Service.Middleware;

public class ApiKeyProtectionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IOptions<ApplicationOptions> _applicationOptions;
	private const string _apiKeyHeaderName = "X-API-Key";

	public ApiKeyProtectionMiddleware(
		RequestDelegate next,
		IOptions<ApplicationOptions> applicationOptions
		)
	{
		_next = next;
		_applicationOptions = applicationOptions;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var endpoint = context.GetEndpoint();
		if (endpoint is null)
		{
			await _next(context);
			return;
		}

		var isAzureHook = endpoint.Metadata.GetMetadata<AzureHookAttribute>() != null;
		if (!isAzureHook)
		{
			await _next(context);
			return;
		}

		// Azure Hook requests must have a "X-API-Key" header.
		if (!context.Request.Headers.TryGetValue(_apiKeyHeaderName, out var apiKey))
		{
			await WriteUnauthorizedAsync(context);
			return;
		}

		await using var scope = context.RequestServices.CreateAsyncScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<DiscordDbContext>();
		var server = await dbContext.Servers.AsNoTracking().FirstOrDefaultAsync(x => x.Key == apiKey.ToString());
		if (server is not null)
		{
			await _next(context);
			return;
		}

		// If no server exists with the configured key, then the header must match the ApiKey from appsettings.json
		// If no ApiKey is configured, then the request cannot proceed.
		if (string.IsNullOrEmpty(_applicationOptions.Value.ApiKey))
		{
			await WriteUnauthorizedAsync(context);
			return;
		}

		if (_applicationOptions.Value.ApiKey.Equals(apiKey))
		{
			await _next(context);
			return;
		}

		await WriteUnauthorizedAsync(context);
	}

	private static async Task WriteUnauthorizedAsync(HttpContext context)
	{
		int statusCode = StatusCodes.Status401Unauthorized;
		ProblemDetails problemDetails = new()
		{
			Title = "API key is missing or invalid.",
			Detail = $"The request must provide a valid '{_apiKeyHeaderName}' HTTP header.",
			Type = $"https://http.cat/{statusCode}",
			Status = statusCode,
			Instance = context.TraceIdentifier,
		};

		context.Response.StatusCode = StatusCodes.Status401Unauthorized;
		await context.Response.WriteAsJsonAsync(problemDetails);
	}
}
