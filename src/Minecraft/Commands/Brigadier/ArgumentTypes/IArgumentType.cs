using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

// There is normal argument types and passthrough argument types, that are just passthrough argument serializers carrying data
/// <summary>Marks either a parsed Brigadier argument type or a protocol-only passthrough argument.</summary>
public interface IAnyArgumentType;

/// <summary>Marks a value produced by parsing a command argument.</summary>
public interface IArgumentValue;

// This is the normal argument type returned by implemented argument serializers
/// <summary>Defines command argument parsing and completion behavior.</summary>
public interface IArgumentType : IAnyArgumentType
{
    /// <summary>Gets representative valid input strings.</summary>
    public IEnumerable<string> Examples { get; }

    /// <summary>Parses a value from the reader's current cursor.</summary>
    /// <param name="reader">The command reader to consume.</param>
    /// <returns>The parsed argument value.</returns>
    public IArgumentValue Parse(StringReader reader);

    /// <summary>Lists completions for this argument type.</summary>
    /// <param name="context">The parsed command context.</param>
    /// <param name="builder">The suggestion builder.</param>
    /// <param name="cancellationToken">A token that may cancel asynchronous providers.</param>
    /// <returns>The suggestions; the default implementation returns <see cref="Suggestions.Empty"/>.</returns>
    public virtual ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Suggestions.Empty);
    }

    /// <summary>Casts this argument type to a requested argument interface or implementation.</summary>
    /// <typeparam name="T">The target argument type.</typeparam>
    /// <returns>This instance cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidCastException">This instance is not assignable to <typeparamref name="T"/>.</exception>
    public T As<T>() where T : IAnyArgumentType
    {
        if (this is not T casted)
            throw new InvalidCastException($"Cannot cast {GetType().Name} to {typeof(T).Name}.");

        return casted;
    }
}
