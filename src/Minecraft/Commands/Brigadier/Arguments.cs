using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Provides concise factories and context accessors for built-in Brigadier argument types.</summary>
public class Arguments
{
    /// <summary>Creates an integer argument with inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The integer argument type.</returns>
    public static IntegerArgumentType Integer(int min = int.MinValue, int max = int.MaxValue)
    {
        return IntegerArgumentType.IntegerArgument(min, max);
    }

    /// <summary>
    /// Gets the parsed integer argument with the specified name from a command context.
    /// </summary>
    /// <param name="context">The command context that contains parsed arguments.</param>
    /// <param name="name">The name of the parsed argument to retrieve.</param>
    /// <returns>The parsed <see cref="int"/> value associated with <paramref name="name"/>.</returns>
    /// <exception cref="System.ArgumentException">
    /// No argument named <paramref name="name"/> exists in <paramref name="context"/>, or the argument was parsed as a type other than <see cref="int"/>.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public static int GetInteger(CommandContext context, string name)
    {
        return context.GetArgument<int>(name);
    }

    /// <summary>Creates a Boolean argument type.</summary>
    /// <returns>The Boolean argument type.</returns>
    public static BoolArgumentType Bool()
    {
        return BoolArgumentType.Bool();
    }

    /// <summary>Gets a parsed Boolean argument.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed value.</returns>
    public static bool GetBool(CommandContext context, string name)
    {
        return context.GetArgument<bool>(name);
    }

    /// <summary>Creates a double argument with inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The double argument type.</returns>
    public static DoubleArgumentType Double(double min = -double.MaxValue, double max = double.MaxValue)
    {
        return DoubleArgumentType.DoubleArgument(min, max);
    }

    /// <summary>Gets a parsed double argument.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed value.</returns>
    public static double GetDouble(CommandContext context, string name)
    {
        return context.GetArgument<double>(name);
    }

    /// <summary>Creates a float argument with inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The float argument type.</returns>
    public static FloatArgumentType Float(float min = -float.MaxValue, float max = float.MaxValue)
    {
        return FloatArgumentType.FloatArgument(min, max);
    }

    /// <summary>Gets a parsed float argument.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed value.</returns>
    public static float GetFloat(CommandContext context, string name)
    {
        return context.GetArgument<float>(name);
    }

    /// <summary>Creates a long argument with inclusive bounds.</summary>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The long argument type.</returns>
    public static LongArgumentType Long(long min = long.MinValue, long max = long.MaxValue)
    {
        return LongArgumentType.LongArgument(min, max);
    }

    /// <summary>Gets a parsed long argument.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed value.</returns>
    public static long GetLong(CommandContext context, string name)
    {
        return context.GetArgument<long>(name);
    }

    /// <summary>Creates a single-word string argument.</summary>
    /// <returns>The string argument type.</returns>
    public static StringArgumentType Word()
    {
        return StringArgumentType.Word();
    }

    /// <summary>Creates a quotable string argument.</summary>
    /// <returns>The string argument type.</returns>
    public static StringArgumentType String()
    {
        return StringArgumentType.String();
    }

    /// <summary>Creates a string argument that consumes the remaining input.</summary>
    /// <returns>The string argument type.</returns>
    public static StringArgumentType GreedyString()
    {
        return StringArgumentType.GreedyString();
    }

    /// <summary>Gets a parsed string argument.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="name">The argument name.</param>
    /// <returns>The parsed value.</returns>
    public static string GetString(CommandContext context, string name)
    {
        return context.GetArgument<string>(name);
    }
}
