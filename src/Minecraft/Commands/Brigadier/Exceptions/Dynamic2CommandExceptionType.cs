using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public class Dynamic2CommandExceptionType(Func<object, object, IMessage> function) : ICommandExceptionType
{
    public CommandSyntaxException Create(object a, object b)
    {
        return new CommandSyntaxException(this, function(a, b));
    }

    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, object a, object b)
    {
        return new CommandSyntaxException(this, function(a, b), new string(reader.Source), reader.Cursor);
    }
}
