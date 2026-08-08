using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

/// <summary>Contains a parsed single-precision floating-point argument.</summary>
/// <param name="Value">The parsed value.</param>
public record FloatArgumentValue(float Value) : IArgumentValue;
/// <summary>Parses single-precision values constrained to an inclusive range.</summary>
public record FloatArgumentType : IArgumentType
{
    /// <inheritdoc/>
    public IEnumerable<string> Examples => ["0", "1.2", ".5", "-1", "-.5", "-1234.56"];

    /// <summary>Gets the inclusive minimum.</summary>
    public required float Minimum { get; init; }
    /// <summary>Gets the inclusive maximum.</summary>
    public required float Maximum { get; init; }

    private FloatArgumentType()
    {
        // Empty
    }

    /// <summary>Creates an argument whose minimum and maximum both evaluate to <see cref="float.MaxValue"/>.</summary>
    /// <returns>The argument type.</returns>
    /// <remarks>The minimum is initialized from <c>-float.MinValue</c>, which equals the positive maximum.</remarks>
    public static FloatArgumentType FloatArgument()
    {
        return FloatArgument(-float.MinValue);
    }

    /// <summary>Creates an argument with a minimum and <see cref="float.MaxValue"/> maximum.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <returns>The argument type.</returns>
    public static FloatArgumentType FloatArgument(float min)
    {
        return FloatArgument(min, float.MaxValue);
    }

    /// <summary>Creates an argument with explicit inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The argument type. The bounds are retained without validation.</returns>
    public static FloatArgumentType FloatArgument(float min, float max)
    {
        return new FloatArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    /// <summary>Gets a parsed float argument from a context.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed single-precision value.</returns>
    public static float GetFloat(CommandContext context, string name)
    {
        return context.GetArgument<float>(name);
    }

    /// <inheritdoc/>
    public IArgumentValue Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadFloat();

        if (result < Minimum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.FloatTooSmall.CreateWithContext(reader, result, Minimum);
        }

        if (result > Maximum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.FloatTooBig.CreateWithContext(reader, result, Maximum);
        }

        return new FloatArgumentValue(result);
    }
}
