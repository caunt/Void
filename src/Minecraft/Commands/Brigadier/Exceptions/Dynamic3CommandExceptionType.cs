using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>
/// Represents a command syntax exception type whose message is created from three runtime arguments.
/// </summary>
/// <param name="function">The factory that receives the values passed to <see cref="Create(object, object, object)"/> or <see cref="CreateWithContext(IImmutableStringReader, object, object, object)"/> and returns the exception message.</param>
public class Dynamic3CommandExceptionType(Func<object, object, object, IMessage> function) : ICommandExceptionType
{
    public CommandSyntaxException Create(object a, object b, object c)
    {
        return new CommandSyntaxException(this, function(a, b, c));
    }

    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, object a, object b, object c)
    {
        return new CommandSyntaxException(this, function(a, b, c), new string(reader.Source), reader.Cursor);
    }
}
