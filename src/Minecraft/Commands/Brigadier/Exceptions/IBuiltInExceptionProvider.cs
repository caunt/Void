namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public interface IBuiltInExceptionProvider
{
    public Dynamic2CommandExceptionType DoubleTooSmall { get; }
    public Dynamic2CommandExceptionType DoubleTooBig { get; }
    public Dynamic2CommandExceptionType FloatTooSmall { get; }
    public Dynamic2CommandExceptionType FloatTooBig { get; }
    public Dynamic2CommandExceptionType IntegerTooSmall { get; }
    public Dynamic2CommandExceptionType IntegerTooBig { get; }
    public Dynamic2CommandExceptionType LongTooSmall { get; }
    public Dynamic2CommandExceptionType LongTooBig { get; }
    public DynamicCommandExceptionType LiteralIncorrect { get; }
    public SimpleCommandExceptionType ReaderExpectedStartOfQuote { get; }
    public SimpleCommandExceptionType ReaderExpectedEndOfQuote { get; }
    public DynamicCommandExceptionType ReaderInvalidEscape { get; }
    public DynamicCommandExceptionType ReaderInvalidBool { get; }
    public DynamicCommandExceptionType ReaderInvalidInt { get; }
    public SimpleCommandExceptionType ReaderExpectedInt { get; }
    public DynamicCommandExceptionType ReaderInvalidLong { get; }
    public SimpleCommandExceptionType ReaderExpectedLong { get; }
    public DynamicCommandExceptionType ReaderInvalidDouble { get; }
    public SimpleCommandExceptionType ReaderExpectedDouble { get; }
    public DynamicCommandExceptionType ReaderInvalidFloat { get; }
    public SimpleCommandExceptionType ReaderExpectedFloat { get; }
    public SimpleCommandExceptionType ReaderExpectedBool { get; }
    public DynamicCommandExceptionType ReaderExpectedSymbol { get; }
    public SimpleCommandExceptionType DispatcherUnknownCommand { get; }
    public SimpleCommandExceptionType DispatcherUnknownArgument { get; }
    public SimpleCommandExceptionType DispatcherExpectedArgumentSeparator { get; }
    public DynamicCommandExceptionType DispatcherParseException { get; }
}
