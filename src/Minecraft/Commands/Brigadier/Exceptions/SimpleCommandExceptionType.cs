namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>Creates command syntax exceptions with a fixed message.</summary>
/// <param name="message">The message shared by created exceptions.</param>
public class SimpleCommandExceptionType(IMessage message) : ICommandExceptionType
{
    /// <summary>Creates an exception without input context.</summary>
    /// <returns>The command syntax exception.</returns>
    public CommandSyntaxException Create()
    {
        return new CommandSyntaxException(this, message);
    }

    /// <summary>Creates an exception at the reader's current cursor.</summary>
    /// <param name="reader">The source and cursor to capture.</param>
    /// <returns>The contextual command syntax exception.</returns>
    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader)
    {
        return new CommandSyntaxException(this, message, reader.Source, reader.Cursor);
    }

    /// <summary>Returns the fixed message text.</summary>
    /// <returns>The message value.</returns>
    public override string ToString()
    {
        return message.Value;
    }
}
