using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

/// <summary>Contains a parsed double-precision floating-point argument.</summary>
/// <param name="Value">The parsed value.</param>
public record DoubleArgumentValue(double Value) : IArgumentValue;
/// <summary>Parses double-precision values constrained to an inclusive range.</summary>
public record DoubleArgumentType : IArgumentType
{
    /// <inheritdoc/>
    public IEnumerable<string> Examples => ["0", "1.2", ".5", "-1", "-.5", "-1234.56"];

    /// <summary>Gets the inclusive minimum.</summary>
    public required double Minimum { get; init; }
    /// <summary>Gets the inclusive maximum.</summary>
    public required double Maximum { get; init; }

    private DoubleArgumentType()
    {
        // Empty
    }

    /// <summary>Creates an argument whose minimum and maximum both evaluate to <see cref="double.MaxValue"/>.</summary>
    /// <returns>The argument type.</returns>
    /// <remarks>The minimum is initialized from <c>-double.MinValue</c>, which equals the positive maximum.</remarks>
    public static DoubleArgumentType DoubleArgument()
    {
        return DoubleArgument(-double.MinValue);
    }

    /// <summary>Creates an argument with a minimum and <see cref="double.MaxValue"/> maximum.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <returns>The argument type.</returns>
    public static DoubleArgumentType DoubleArgument(double min)
    {
        return DoubleArgument(min, double.MaxValue);
    }

    /// <summary>Creates an argument with explicit inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The argument type. The bounds are retained without validation.</returns>
    public static DoubleArgumentType DoubleArgument(double min, double max)
    {
        return new DoubleArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    /// <summary>Gets a parsed double argument from a context.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed double-precision value.</returns>
    public static double GetDouble(CommandContext context, string name)
    {
        return context.GetArgument<double>(name);
    }

    /// <inheritdoc/>
    public IArgumentValue Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadDouble();

        if (result < Minimum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.DoubleTooSmall.CreateWithContext(reader, result, Minimum);
        }

        if (result > Maximum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.DoubleTooBig.CreateWithContext(reader, result, Maximum);
        }

        return new DoubleArgumentValue(result);
    }
}
