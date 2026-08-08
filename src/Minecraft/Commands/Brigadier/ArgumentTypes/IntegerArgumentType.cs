using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

/// <summary>Contains a parsed signed 32-bit integer argument.</summary>
/// <param name="Value">The parsed value.</param>
public record IntegerArgumentValue(int Value) : IArgumentValue;
/// <summary>Parses signed 32-bit integers constrained to an inclusive range.</summary>
public record IntegerArgumentType : IArgumentType
{
    /// <inheritdoc/>
    public IEnumerable<string> Examples => ["0", "123", "-123"];

    /// <summary>Gets the inclusive minimum.</summary>
    public required int Minimum { get; init; }
    /// <summary>Gets the inclusive maximum.</summary>
    public required int Maximum { get; init; }

    private IntegerArgumentType()
    {
        // Empty
    }

    /// <summary>Creates an argument accepting the complete 32-bit integer range.</summary>
    /// <returns>The argument type.</returns>
    public static IntegerArgumentType IntegerArgument()
    {
        return IntegerArgument(int.MinValue);
    }

    /// <summary>Creates an argument with a minimum and no narrower maximum.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <returns>The argument type.</returns>
    public static IntegerArgumentType IntegerArgument(int min)
    {
        return IntegerArgument(min, int.MaxValue);
    }

    /// <summary>Creates an argument with explicit inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The argument type. The bounds are retained without validation.</returns>
    public static IntegerArgumentType IntegerArgument(int min, int max)
    {
        return new IntegerArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    /// <summary>Gets a parsed integer argument from a context.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed integer.</returns>
    public static int GetInteger(CommandContext context, string name)
    {
        return context.GetArgument<int>(name);
    }

    /// <inheritdoc/>
    public IArgumentValue Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadInt();

        if (result < Minimum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.IntegerTooSmall.CreateWithContext(reader, result, Minimum);
        }

        if (result > Maximum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.IntegerTooBig.CreateWithContext(reader, result, Maximum);
        }

        return new IntegerArgumentValue(result);
    }
}
