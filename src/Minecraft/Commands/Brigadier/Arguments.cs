using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier;

public class Arguments
{
    public static IntegerArgumentType Integer(int min = int.MinValue, int max = int.MaxValue)
    {
        return IntegerArgumentType.IntegerArgument(min, max);
    }

    public static int GetInteger(CommandContext context, string name)
    {
        return context.GetArgument<int>(name);
    }

    public static BoolArgumentType Bool()
    {
        return BoolArgumentType.Bool();
    }

    public static bool GetBool(CommandContext context, string name)
    {
        return context.GetArgument<bool>(name);
    }

    public static DoubleArgumentType Double(double min = -double.MaxValue, double max = double.MaxValue)
    {
        return DoubleArgumentType.DoubleArgument(min, max);
    }

    public static double GetDouble(CommandContext context, string name)
    {
        return context.GetArgument<double>(name);
    }

    public static FloatArgumentType Float(float min = -float.MaxValue, float max = float.MaxValue)
    {
        return FloatArgumentType.FloatArgument(min, max);
    }

    public static float GetFloat(CommandContext context, string name)
    {
        return context.GetArgument<float>(name);
    }

    public static LongArgumentType Long(long min = long.MinValue, long max = long.MaxValue)
    {
        return LongArgumentType.LongArgument(min, max);
    }

    public static long GetLong(CommandContext context, string name)
    {
        return context.GetArgument<long>(name);
    }

    public static StringArgumentType Word()
    {
        return StringArgumentType.Word();
    }

    public static StringArgumentType String()
    {
        return StringArgumentType.String();
    }

    public static StringArgumentType GreedyString()
    {
        return StringArgumentType.GreedyString();
    }

    public static string GetString(CommandContext context, string name)
    {
        return context.GetArgument<string>(name);
    }
}
