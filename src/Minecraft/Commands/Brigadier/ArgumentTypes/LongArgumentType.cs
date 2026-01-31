using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

public record LongArgumentValue(long Value) : IArgumentValue;
public record LongArgumentType : IArgumentType
{
    public IEnumerable<string> Examples => ["0", "123", "-123"];

    public required long Minimum { get; init; }
    public required long Maximum { get; init; }

    private LongArgumentType()
    {
        // Empty
    }

    public static LongArgumentType LongArgument()
    {
        return LongArgument(long.MinValue);
    }

    public static LongArgumentType LongArgument(long min)
    {
        return LongArgument(min, long.MaxValue);
    }

    public static LongArgumentType LongArgument(long min, long max)
    {
        return new LongArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    public static long GetLong(CommandContext context, string name)
    {
        return context.GetArgument<long>(name);
    }

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
