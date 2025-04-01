using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

public delegate ValueTask<Suggestions> SuggestionProvider(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken);

public delegate Suggestions SuggestionProviderSync(CommandContext context, SuggestionsBuilder builder);
