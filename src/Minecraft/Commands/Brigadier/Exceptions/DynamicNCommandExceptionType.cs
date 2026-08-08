using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>Creates command syntax exceptions whose messages depend on an arbitrary value array.</summary>
/// <param name="function">The message factory.</param>
public class DynamicNCommandExceptionType(Func<object[], IMessage> function) : ICommandExceptionType
{
    /// <summary>Creates an exception without input context.</summary>
    /// <param name="objects">The values passed as one array to the message factory.</param>
    /// <returns>The command syntax exception.</returns>
    public CommandSyntaxException Create(params object[] objects)
    {
        return new CommandSyntaxException(this, function(objects));
    }

    /// <summary>Creates an exception at the reader's current cursor.</summary>
    /// <param name="reader">The source and cursor to capture.</param>
    /// <param name="objects">The values passed as one array to the message factory.</param>
    /// <returns>The contextual exception.</returns>
    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, params object[] objects)
    {
        return new CommandSyntaxException(this, function(objects), new string(reader.Source), reader.Cursor);
    }
}
