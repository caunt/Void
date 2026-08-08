using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>
/// Represents a command syntax exception type whose message is created from three runtime arguments.
/// </summary>
/// <param name="function">The factory that receives the values passed to <see cref="Create(object, object, object)"/> or <see cref="CreateWithContext(IImmutableStringReader, object, object, object)"/> and returns the exception message.</param>
public class Dynamic3CommandExceptionType(Func<object, object, object, IMessage> function) : ICommandExceptionType
{
    /// <summary>Creates an exception without input context.</summary>
    /// <param name="a">The first message value.</param>
    /// <param name="b">The second message value.</param>
    /// <param name="c">The third message value.</param>
    /// <returns>The command syntax exception.</returns>
    public CommandSyntaxException Create(object a, object b, object c)
    {
        return new CommandSyntaxException(this, function(a, b, c));
    }

    /// <summary>Creates an exception at the reader's current cursor.</summary>
    /// <param name="reader">The source and cursor to capture.</param>
    /// <param name="a">The first message value.</param>
    /// <param name="b">The second message value.</param>
    /// <param name="c">The third message value.</param>
    /// <returns>The contextual exception.</returns>
    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, object a, object b, object c)
    {
        return new CommandSyntaxException(this, function(a, b, c), new string(reader.Source), reader.Cursor);
    }
}
