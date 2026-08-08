using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>Creates command syntax exceptions whose messages depend on one runtime value.</summary>
/// <param name="function">The message factory.</param>
public class DynamicCommandExceptionType(Func<object, IMessage> function) : ICommandExceptionType
{
    /// <summary>Creates an exception without input context.</summary>
    /// <param name="value">The value passed to the message factory.</param>
    /// <returns>The command syntax exception.</returns>
    public CommandSyntaxException Create(object value)
    {
        return new CommandSyntaxException(this, function(value));
    }

    /// <summary>Creates an exception at the reader's current cursor.</summary>
    /// <param name="reader">The source and cursor to capture.</param>
    /// <param name="value">The value passed to the message factory.</param>
    /// <returns>The contextual exception.</returns>
    public CommandSyntaxException CreateWithContext(StringReader reader, object value)
    {
        return new CommandSyntaxException(this, function(value), reader.Source, reader.Cursor);
    }
}
