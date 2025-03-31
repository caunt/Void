using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public class DynamicCommandExceptionType(Func<object, IMessage> function) : ICommandExceptionType
{
    public CommandSyntaxException Create(object value)
    {
        return new CommandSyntaxException(this, function(value));
    }

    public CommandSyntaxException CreateWithContext(StringReader reader, object value)
    {
        return new CommandSyntaxException(this, function(value), reader.Source, reader.Cursor);
    }
}
