using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

/// <summary>Contains a parsed signed 64-bit integer argument.</summary>
/// <param name="Value">The parsed value.</param>
public record LongArgumentValue(long Value) : IArgumentValue;
/// <summary>Constrains a numeric token to an inclusive 64-bit range.</summary>
/// <remarks>The current parser consumes the token through <see cref="StringReader.ReadInt"/>, so values outside the 32-bit range fail before the 64-bit bounds are checked.</remarks>
public record LongArgumentType : IArgumentType
{
    /// <inheritdoc/>
    public IEnumerable<string> Examples => ["0", "123", "-123"];

    /// <summary>Gets the inclusive minimum.</summary>
    public required long Minimum { get; init; }
    /// <summary>Gets the inclusive maximum.</summary>
    public required long Maximum { get; init; }

    private LongArgumentType()
    {
        // Empty
    }

    /// <summary>Creates an argument configured with the complete 64-bit range.</summary>
    /// <returns>The argument type.</returns>
    public static LongArgumentType LongArgument()
    {
        return LongArgument(long.MinValue);
    }

    /// <summary>Creates an argument with a minimum and <see cref="long.MaxValue"/> maximum.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <returns>The argument type.</returns>
    public static LongArgumentType LongArgument(long min)
    {
        return LongArgument(min, long.MaxValue);
    }

    /// <summary>Creates an argument with explicit inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The argument type. The bounds are retained without validation.</returns>
    public static LongArgumentType LongArgument(long min, long max)
    {
        return new LongArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    /// <summary>Gets a parsed long argument from a context.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed long integer.</returns>
    public static long GetLong(CommandContext context, string name)
    {
        return context.GetArgument<long>(name);
    }

    /// <inheritdoc/>
    public IArgumentValue Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadInt();

        if (result < Minimum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.LongTooSmall.CreateWithContext(reader, result, Minimum);
        }

        if (result > Maximum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.LongTooBig.CreateWithContext(reader, result, Maximum);
        }

        return new LongArgumentValue(result);
    }
}
