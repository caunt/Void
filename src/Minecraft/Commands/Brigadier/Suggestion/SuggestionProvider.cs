using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

/// <summary>Produces asynchronous command completions for an argument node.</summary>
/// <param name="context">The parsed command context.</param>
/// <param name="builder">The suggestion accumulator.</param>
/// <param name="cancellationToken">A token that may cancel the operation.</param>
/// <returns>The completed suggestions.</returns>
public delegate ValueTask<Suggestions> SuggestionProvider(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken);

/// <summary>Produces synchronous command completions for an argument node.</summary>
/// <param name="context">The parsed command context.</param>
/// <param name="builder">The suggestion accumulator.</param>
/// <returns>The completed suggestions.</returns>
public delegate Suggestions SuggestionProviderSync(CommandContext context, SuggestionsBuilder builder);
