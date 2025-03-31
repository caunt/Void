using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

public interface IArgumentType<TType>
{
    public IEnumerable<string> Examples { get; }

    public TType Parse(StringReader reader);

    public virtual TType Parse(StringReader reader, ICommandSource source)
    {
        return Parse(reader);
    }

    public virtual ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Suggestions.Empty);
    }
}
