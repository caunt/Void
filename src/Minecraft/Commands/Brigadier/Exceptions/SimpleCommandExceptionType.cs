namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public class SimpleCommandExceptionType(IMessage message) : ICommandExceptionType
{
    public CommandSyntaxException Create()
    {
        return new CommandSyntaxException(this, message);
    }

    public CommandSyntaxException CreateWithContext(IImmutableStringReader reader)
    {
        return new CommandSyntaxException(this, message, reader.Source, reader.Cursor);
    }

    public override string ToString()
    {
        return message.Value;
    }
}
