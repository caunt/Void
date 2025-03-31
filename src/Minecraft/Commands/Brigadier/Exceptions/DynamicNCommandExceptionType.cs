using System;

namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public class DynamicNCommandExceptionType(Func<object[], IMessage> function) : ICommandExceptionType
{
    public CommandSyntaxException Create(params object[] objects)
    {
        return new CommandSyntaxException(this, function(objects));
    }

    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, params object[] objects)
    {
        return new CommandSyntaxException(this, function(objects), new string(reader.Source), reader.Cursor);
    }
}
