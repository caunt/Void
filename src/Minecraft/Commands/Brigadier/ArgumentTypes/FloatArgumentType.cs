using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

public record FloatArgumentType : IArgumentType<float>
{
    public IEnumerable<string> Examples => ["0", "1.2", ".5", "-1", "-.5", "-1234.56"];

    public required float Minimum { get; init; }
    public required float Maximum { get; init; }

    private FloatArgumentType()
    {
        // Empty
    }

    public static FloatArgumentType FloatArgument()
    {
        return FloatArgument(-float.MinValue);
    }

    public static FloatArgumentType FloatArgument(float min)
    {
        return FloatArgument(min, float.MaxValue);
    }

    public static FloatArgumentType FloatArgument(float min, float max)
    {
        return new FloatArgumentType
        {
            Minimum = min,
            Maximum = max
        };
    }

    public static float GetFloat(CommandContext context, string name)
    {
        return context.GetArgument<float>(name);
    }

    public float Parse(StringReader reader)
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

        return result;
    }
}
