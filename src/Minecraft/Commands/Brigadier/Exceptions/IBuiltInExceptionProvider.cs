namespace Void.Minecraft.Commands.Brigadier.Exceptions;

/// <summary>Provides the standard command parser and dispatcher exception factories.</summary>
public interface IBuiltInExceptionProvider
{
    /// <summary>Gets the exception type for a double below its minimum.</summary>
    public Dynamic2CommandExceptionType DoubleTooSmall { get; }
    /// <summary>Gets the exception type for a double above its maximum.</summary>
    public Dynamic2CommandExceptionType DoubleTooBig { get; }
    /// <summary>Gets the exception type for a float below its minimum.</summary>
    public Dynamic2CommandExceptionType FloatTooSmall { get; }
    /// <summary>Gets the exception type for a float above its maximum.</summary>
    public Dynamic2CommandExceptionType FloatTooBig { get; }
    /// <summary>Gets the exception type for an integer below its minimum.</summary>
    public Dynamic2CommandExceptionType IntegerTooSmall { get; }
    /// <summary>Gets the exception type for an integer above its maximum.</summary>
    public Dynamic2CommandExceptionType IntegerTooBig { get; }
    /// <summary>Gets the exception type for a long below its minimum.</summary>
    public Dynamic2CommandExceptionType LongTooSmall { get; }
    /// <summary>Gets the exception type for a long above its maximum.</summary>
    public Dynamic2CommandExceptionType LongTooBig { get; }
    /// <summary>Gets the exception type for a mismatched literal.</summary>
    public DynamicCommandExceptionType LiteralIncorrect { get; }
    /// <summary>Gets the exception type for a missing opening quote.</summary>
    public SimpleCommandExceptionType ReaderExpectedStartOfQuote { get; }
    /// <summary>Gets the exception type for an unclosed quoted string.</summary>
    public SimpleCommandExceptionType ReaderExpectedEndOfQuote { get; }
    /// <summary>Gets the exception type for an invalid quoted-string escape.</summary>
    public DynamicCommandExceptionType ReaderInvalidEscape { get; }
    /// <summary>Gets the exception type for an invalid Boolean token.</summary>
    public DynamicCommandExceptionType ReaderInvalidBool { get; }
    /// <summary>Gets the exception type for an invalid integer token.</summary>
    public DynamicCommandExceptionType ReaderInvalidInt { get; }
    /// <summary>Gets the exception type for a missing integer.</summary>
    public SimpleCommandExceptionType ReaderExpectedInt { get; }
    /// <summary>Gets the exception type for an invalid long token.</summary>
    public DynamicCommandExceptionType ReaderInvalidLong { get; }
    /// <summary>Gets the exception type for a missing long.</summary>
    public SimpleCommandExceptionType ReaderExpectedLong { get; }
    /// <summary>Gets the exception type for an invalid double token.</summary>
    public DynamicCommandExceptionType ReaderInvalidDouble { get; }
    /// <summary>Gets the exception type for a missing double.</summary>
    public SimpleCommandExceptionType ReaderExpectedDouble { get; }
    /// <summary>Gets the exception type for an invalid float token.</summary>
    public DynamicCommandExceptionType ReaderInvalidFloat { get; }
    /// <summary>Gets the exception type for a missing float.</summary>
    public SimpleCommandExceptionType ReaderExpectedFloat { get; }
    /// <summary>Gets the exception type for a missing Boolean.</summary>
    public SimpleCommandExceptionType ReaderExpectedBool { get; }
    /// <summary>Gets the exception type for a missing expected symbol.</summary>
    public DynamicCommandExceptionType ReaderExpectedSymbol { get; }
    /// <summary>Gets the exception type for an unknown command.</summary>
    public SimpleCommandExceptionType DispatcherUnknownCommand { get; }
    /// <summary>Gets the exception type for an invalid command argument.</summary>
    public SimpleCommandExceptionType DispatcherUnknownArgument { get; }
    /// <summary>Gets the exception type for missing whitespace after an argument.</summary>
    public SimpleCommandExceptionType DispatcherExpectedArgumentSeparator { get; }
    /// <summary>Gets the exception type used to wrap an arbitrary dispatcher parse failure.</summary>
    public DynamicCommandExceptionType DispatcherParseException { get; }
}
