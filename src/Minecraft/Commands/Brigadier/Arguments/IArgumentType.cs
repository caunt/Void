using System.Collections.Generic;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.Arguments;

public interface IArgumentType<TType>
{
    public IEnumerable<string> Examples { get; }

    public TType Parse(StringReader reader);

    public virtual TType Parse(StringReader reader, ICommandSource source)
    {
        return Parse(reader);
    }

    public virtual ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder)
    {
        return ValueTask.FromResult(Suggestions.Empty);
    }
}
