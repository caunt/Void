using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

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
