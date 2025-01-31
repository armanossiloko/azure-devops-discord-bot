using Discord.Interactions;

namespace Azure.Discord.Bot.Service.Handlers;

public class AzureDevOpsEventTypesAutocompleteHandler : AutocompleteHandler
{
	private static readonly List<string> _eventTypes = new()
	{
		//"workitem.created",
		//"workitem.updated",
		//"workitem.deleted",

		// Code pushed to a repository
		"git.push",
		// PR created
		"git.pullrequest.created",
		// PR updated
		"git.pullrequest.updated",
		// PR merge attempted
		"git.pullrequest.merged",
		// Commented on
		"ms.vss-code.git-pullrequest-comment-event",

		//"build.created",
		//"build.updated",
		//"release.created",
		//"release.updated",
		//"code.push",
		//"code.pullrequest",
		//"test.result.published",
		//"project.created",
		//"project.updated",
		//"repository.created",
		//"repository.updated",
		//"artifact.published",
		//"agent.job.completed",
		//"subscription.created",
		//"subscription.updated",
		//"subscription.deleted"
	};

	public override Task<AutocompletionResult> GenerateSuggestionsAsync(
		IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction,
		IParameterInfo parameter,
		IServiceProvider services
		)
	{
		var results = _eventTypes.Select(type => new AutocompleteResult(type, type)).ToList();
		return Task.FromResult(AutocompletionResult.FromSuccess(results));
	}
}