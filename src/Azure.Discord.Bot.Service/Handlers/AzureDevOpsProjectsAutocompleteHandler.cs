using Azure.Discord.Bot.DataAccess;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Azure.Discord.Bot.Service.Handlers;

public class AzureDevOpsProjectsAutocompleteHandler : AutocompleteHandler
{
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
		IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction,
		IParameterInfo parameter,
		IServiceProvider services
		)
	{
		await using var scope = services.CreateAsyncScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<DiscordDbContext>();
		var linkedOrganizations = await dbContext
			.LinkedOrganizations
			.AsNoTracking()
			.Where(x => x.ServerId == context.Guild.Id)
			.ToListAsync();

		Models.LinkedServerOrganization? organization;
		if (linkedOrganizations.Count == 1)
		{
			organization = linkedOrganizations.Single();
		}
		else
		{
			var interactionData = autocompleteInteraction.Data.Options;
			var organizationName = interactionData.FirstOrDefault(x => x.Name == "organization-name")?.Value?.ToString();
			organization = linkedOrganizations.FirstOrDefault(x => x.DisplayName == organizationName);
		}

		if (organization is null)
		{
			string error = linkedOrganizations.Count == 0
				? "You have no organization linked / configured."
				: "You must specify an organization first.";
			return AutocompletionResult.FromError(InteractionCommandError.UnmetPrecondition, error);
		}

		var credentials = new VssBasicCredential(string.Empty, organization.Token);
		using var connection = new VssConnection(new Uri(organization.OrganizationUrl), credentials);

		var projectHttpClient = connection.GetClient<ProjectHttpClient>();
		var projects = await projectHttpClient.GetProjects();

		var results = projects.Select(type => new AutocompleteResult(type.Name, type.Id)).ToList();
		return AutocompletionResult.FromSuccess(results);
	}
}