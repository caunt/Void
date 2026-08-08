using Void.Minecraft.Commands.Brigadier.ArgumentTypes;

namespace Void.Minecraft.Commands.Brigadier.Context;

/// <summary>Exposes the typed value produced by an argument parser.</summary>
public interface IParsedArgument
{
    /// <summary>Gets the parsed argument value.</summary>
    public IArgumentValue Result { get; }
}

/// <summary>Associates a parsed argument value with its input range.</summary>
/// <param name="Start">The inclusive start cursor.</param>
/// <param name="End">The exclusive end cursor.</param>
/// <param name="Result">The parsed value.</param>
public record ParsedArgument(int Start, int End, IArgumentValue Result) : IParsedArgument
{
    /// <summary>Gets the range from <see cref="Start"/> through <see cref="End"/>.</summary>
    public StringRange Range { get; } = new(Start, End);
}
