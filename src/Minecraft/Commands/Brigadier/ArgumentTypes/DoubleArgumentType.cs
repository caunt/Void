using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

public record DoubleArgumentValue(double Value) : IArgumentValue;
public record DoubleArgumentType : IArgumentType
{
    public IEnumerable<string> Examples => ["0", "1.2", ".5", "-1", "-.5", "-1234.56"];

    public required double Minimum { get; init; }
    public required double Maximum { get; init; }

    private DoubleArgumentType()
    {
        // Empty
    }

    public static DoubleArgumentType DoubleArgument()
    {
        return DoubleArgument(-double.MinValue);
    }

    public static DoubleArgumentType DoubleArgument(double min)
    {
        return DoubleArgument(min, double.MaxValue);
    }

    public static DoubleArgumentType DoubleArgument(double min, double max)
    {
        return new DoubleArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    public static double GetDouble(CommandContext context, string name)
    {
        return context.GetArgument<double>(name);
    }

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
