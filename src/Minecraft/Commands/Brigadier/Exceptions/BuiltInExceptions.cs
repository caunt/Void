namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>Provides English-language implementations of Brigadier's standard exceptions.</summary>
public class BuiltInExceptions : IBuiltInExceptionProvider
{
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType DoubleTooSmall { get; } = new((found, min) => new LiteralMessage($"Double must not be less than {min}, found {found}"));
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType DoubleTooBig { get; } = new((found, max) => new LiteralMessage($"Double must not be more than {max}, found {found}"));
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType FloatTooSmall { get; } = new((found, min) => new LiteralMessage($"Float must not be less than {min}, found {found}"));
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType FloatTooBig { get; } = new((found, max) => new LiteralMessage("Float must not be more than " + max + ", found " + found));
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType IntegerTooSmall { get; } = new((found, min) => new LiteralMessage("Integer must not be less than " + min + ", found " + found));
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType IntegerTooBig { get; } = new((found, max) => new LiteralMessage("Integer must not be more than " + max + ", found " + found));
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType LongTooSmall { get; } = new((found, min) => new LiteralMessage("Long must not be less than " + min + ", found " + found));
    /// <inheritdoc/>
    public Dynamic2CommandExceptionType LongTooBig { get; } = new((found, max) => new LiteralMessage("Long must not be more than " + max + ", found " + found));
    /// <inheritdoc/>
    public DynamicCommandExceptionType LiteralIncorrect { get; } = new(expected => new LiteralMessage("Expected literal " + expected));
    /// <inheritdoc/>
    public SimpleCommandExceptionType ReaderExpectedStartOfQuote { get; } = new(new LiteralMessage("Expected quote to start a string"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType ReaderExpectedEndOfQuote { get; } = new(new LiteralMessage("Unclosed quoted string"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType ReaderInvalidEscape { get; } = new(character => new LiteralMessage("Invalid escape sequence '" + character + "' in quoted string"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType ReaderInvalidBool { get; } = new(value => new LiteralMessage("Invalid bool, expected true or false but found '" + value + "'"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType ReaderInvalidInt { get; } = new(value => new LiteralMessage("Invalid integer '" + value + "'"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType ReaderExpectedInt { get; } = new(new LiteralMessage("Expected integer"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType ReaderInvalidLong { get; } = new(value => new LiteralMessage("Invalid long '" + value + "'"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType ReaderExpectedLong { get; } = new(new LiteralMessage("Expected long"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType ReaderInvalidDouble { get; } = new(value => new LiteralMessage("Invalid double '" + value + "'"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType ReaderExpectedDouble { get; } = new(new LiteralMessage("Expected double"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType ReaderInvalidFloat { get; } = new(value => new LiteralMessage("Invalid float '" + value + "'"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType ReaderExpectedFloat { get; } = new(new LiteralMessage("Expected float"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType ReaderExpectedBool { get; } = new(new LiteralMessage("Expected bool"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType ReaderExpectedSymbol { get; } = new(symbol => new LiteralMessage("Expected '" + symbol + "'"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType DispatcherUnknownCommand { get; } = new(new LiteralMessage("Unknown command"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType DispatcherUnknownArgument { get; } = new(new LiteralMessage("Incorrect argument for command"));
    /// <inheritdoc/>
    public SimpleCommandExceptionType DispatcherExpectedArgumentSeparator { get; } = new(new LiteralMessage("Expected whitespace to end one argument, but found trailing data"));
    /// <inheritdoc/>
    public DynamicCommandExceptionType DispatcherParseException { get; } = new(message => new LiteralMessage("Could not parse command: " + message));
}
