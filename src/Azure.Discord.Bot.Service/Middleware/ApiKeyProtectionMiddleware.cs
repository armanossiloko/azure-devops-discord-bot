using Azure.Discord.Bot.Service.Configuration;
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
		if (string.IsNullOrEmpty(_applicationOptions.Value.ApiKey))
		{
			await _next(context);
			return;
		}

		if (context.Request.Headers.TryGetValue(_apiKeyHeaderName, out var apiKey)
			&& _applicationOptions.Value.ApiKey.Equals(apiKey))
		{
			await _next(context);
			return;
			
		}

		// TODO: Implement a different JSON body to return
		var jsonResponse = new
		{
			status = "error",
			message = "API key is missing or invalid."
		};

		context.Response.StatusCode = StatusCodes.Status401Unauthorized;
		await context.Response.WriteAsJsonAsync(jsonResponse);
		return;
	}

}
