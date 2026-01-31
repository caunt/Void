using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

// There is normal argument types and passthrough argument types, that are just passthrough argument serializers carrying data
public interface IAnyArgumentType;

public interface IArgumentValue;

// This is the normal argument type returned by implemented argument serializers
public interface IArgumentType : IAnyArgumentType
{
    public IEnumerable<string> Examples { get; }

    public IArgumentValue Parse(StringReader reader);

    public virtual ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Suggestions.Empty);
    }

    public T As<T>() where T : IAnyArgumentType
    {
        if (this is not T casted)
            throw new InvalidCastException($"Cannot cast {GetType().Name} to {typeof(T).Name}.");

        return casted;
    }
}
