using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public class Dynamic4CommandExceptionType(Func<object, object, object, object, IMessage> function) : ICommandExceptionType
{
    public CommandSyntaxException Create(object a, object b, object c, object d)
    {
        return new CommandSyntaxException(this, function(a, b, c, d));
    }

    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, object a, object b, object c, object d)
    {
        return new CommandSyntaxException(this, function(a, b, c, d), new string(reader.Source), reader.Cursor);
    }
}
