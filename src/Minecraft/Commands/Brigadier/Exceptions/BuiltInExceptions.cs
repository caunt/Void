namespace Void.Minecraft.Commands.Brigadier.Exceptions;

public class BuiltInExceptions : IBuiltInExceptionProvider
{
    public Dynamic2CommandExceptionType DoubleTooSmall { get; } = new((found, min) => new LiteralMessage($"Double must not be less than {min}, found {found}"));
    public Dynamic2CommandExceptionType DoubleTooBig { get; } = new((found, max) => new LiteralMessage("Double must not be more than " + max + ", found " + found));
    public Dynamic2CommandExceptionType FloatTooSmall { get; } = new((found, min) => new LiteralMessage("Float must not be less than " + min + ", found " + found));
    public Dynamic2CommandExceptionType FloatTooBig { get; } = new((found, max) => new LiteralMessage("Float must not be more than " + max + ", found " + found));
    public Dynamic2CommandExceptionType IntegerTooSmall { get; } = new((found, min) => new LiteralMessage("Integer must not be less than " + min + ", found " + found));
    public Dynamic2CommandExceptionType IntegerTooBig { get; } = new((found, max) => new LiteralMessage("Integer must not be more than " + max + ", found " + found));
    public Dynamic2CommandExceptionType LongTooSmall { get; } = new((found, min) => new LiteralMessage("Long must not be less than " + min + ", found " + found));
    public Dynamic2CommandExceptionType LongTooBig { get; } = new((found, max) => new LiteralMessage("Long must not be more than " + max + ", found " + found));
    public DynamicCommandExceptionType LiteralIncorrect { get; } = new(expected => new LiteralMessage("Expected literal " + expected));
    public SimpleCommandExceptionType ReaderExpectedStartOfQuote { get; } = new(new LiteralMessage("Expected quote to start a string"));
    public SimpleCommandExceptionType ReaderExpectedEndOfQuote { get; } = new(new LiteralMessage("Unclosed quoted string"));
    public DynamicCommandExceptionType ReaderInvalidEscape { get; } = new(character => new LiteralMessage("Invalid escape sequence '" + character + "' in quoted string"));
    public DynamicCommandExceptionType ReaderInvalidBool { get; } = new(value => new LiteralMessage("Invalid bool, expected true or false but found '" + value + "'"));
    public DynamicCommandExceptionType ReaderInvalidInt { get; } = new(value => new LiteralMessage("Invalid integer '" + value + "'"));
    public SimpleCommandExceptionType ReaderExpectedInt { get; } = new(new LiteralMessage("Expected integer"));
    public DynamicCommandExceptionType ReaderInvalidLong { get; } = new(value => new LiteralMessage("Invalid long '" + value + "'"));
    public SimpleCommandExceptionType ReaderExpectedLong { get; } = new(new LiteralMessage("Expected long"));
    public DynamicCommandExceptionType ReaderInvalidDouble { get; } = new(value => new LiteralMessage("Invalid double '" + value + "'"));
    public SimpleCommandExceptionType ReaderExpectedDouble { get; } = new(new LiteralMessage("Expected double"));
    public DynamicCommandExceptionType ReaderInvalidFloat { get; } = new(value => new LiteralMessage("Invalid float '" + value + "'"));
    public SimpleCommandExceptionType ReaderExpectedFloat { get; } = new(new LiteralMessage("Expected float"));
    public SimpleCommandExceptionType ReaderExpectedBool { get; } = new(new LiteralMessage("Expected bool"));
    public DynamicCommandExceptionType ReaderExpectedSymbol { get; } = new(symbol => new LiteralMessage("Expected '" + symbol + "'"));
    public SimpleCommandExceptionType DispatcherUnknownCommand { get; } = new(new LiteralMessage("Unknown command"));
    public SimpleCommandExceptionType DispatcherUnknownArgument { get; } = new(new LiteralMessage("Incorrect argument for command"));
    public SimpleCommandExceptionType DispatcherExpectedArgumentSeparator { get; } = new(new LiteralMessage("Expected whitespace to end one argument, but found trailing data"));
    public DynamicCommandExceptionType DispatcherParseException { get; } = new(message => new LiteralMessage("Could not parse command: " + message));
}
