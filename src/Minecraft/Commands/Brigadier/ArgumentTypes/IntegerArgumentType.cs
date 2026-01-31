using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

public record IntegerArgumentValue(int Value) : IArgumentValue;
public record IntegerArgumentType : IArgumentType
{
    public IEnumerable<string> Examples => ["0", "123", "-123"];

    public required int Minimum { get; init; }
    public required int Maximum { get; init; }

    private IntegerArgumentType()
    {
        // Empty
    }

    public static IntegerArgumentType IntegerArgument()
    {
        return IntegerArgument(int.MinValue);
    }

    public static IntegerArgumentType IntegerArgument(int min)
    {
        return IntegerArgument(min, int.MaxValue);
    }

    public static IntegerArgumentType IntegerArgument(int min, int max)
    {
        return new IntegerArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    public static int GetInteger(CommandContext context, string name)
    {
        return context.GetArgument<int>(name);
    }

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
