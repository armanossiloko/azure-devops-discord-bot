using Azure.Discord.Bot.DataAccess;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Azure.Discord.Bot.Service.Handlers;

/// <summary>
/// Autocomplete handler which returns a list linked organizations for a specific guild / server.
/// </summary>
public class LinkedOrganizationsAutocompleteHandler : AutocompleteHandler
{
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
		IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction,
		IParameterInfo parameter,
		IServiceProvider services
		)
	{
		await using var scope = services.CreateAsyncScope();
		var _context = scope.ServiceProvider.GetRequiredService<DiscordDbContext>();
		var guildId = context.Guild.Id;
		var linkedOrgs = await _context.LinkedOrganizations.AsNoTracking().Where(x => x.ServerId == guildId).ToListAsync();
		var results = linkedOrgs.Select(l => new AutocompleteResult(l.DisplayName, l.DisplayName)).ToList();
		return AutocompletionResult.FromSuccess(results);
	}
}
