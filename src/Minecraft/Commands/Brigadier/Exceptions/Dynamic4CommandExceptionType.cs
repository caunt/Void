using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>Creates command syntax exceptions whose messages depend on four runtime values.</summary>
/// <param name="function">The message factory.</param>
public class Dynamic4CommandExceptionType(Func<object, object, object, object, IMessage> function) : ICommandExceptionType
{
    /// <summary>Creates an exception without input context.</summary>
    /// <param name="a">The first message value.</param>
    /// <param name="b">The second message value.</param>
    /// <param name="c">The third message value.</param>
    /// <param name="d">The fourth message value.</param>
    /// <returns>The command syntax exception.</returns>
    public CommandSyntaxException Create(object a, object b, object c, object d)
    {
        return new CommandSyntaxException(this, function(a, b, c, d));
    }

    /// <summary>Creates an exception at the reader's current cursor.</summary>
    /// <param name="reader">The source and cursor to capture.</param>
    /// <param name="a">The first message value.</param>
    /// <param name="b">The second message value.</param>
    /// <param name="c">The third message value.</param>
    /// <param name="d">The fourth message value.</param>
    /// <returns>The contextual exception.</returns>
    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, object a, object b, object c, object d)
    {
        return new CommandSyntaxException(this, function(a, b, c, d), new string(reader.Source), reader.Cursor);
    }
}
