using System;
using System.Text;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Provides cursor-based parsing primitives for Brigadier command input.</summary>
/// <param name="source">The complete command input.</param>
/// <param name="cursor">The initial zero-based cursor.</param>
public class StringReader(string source, int cursor = 0) : IImmutableStringReader
{
    private const char SyntaxEscape = '\\';
    private const char SyntaxDoubleQuote = '"';
    private const char SyntaxSingleQuote = '\'';

    /// <inheritdoc/>
    public string Source { get; init; } = source;
    /// <inheritdoc/>
    public int Cursor { get; set; } = cursor;
    /// <inheritdoc/>
    public int RemainingLength => TotalLength - Cursor;
    /// <inheritdoc/>
    public int TotalLength => Source.Length;
    /// <inheritdoc/>
    public string Read => Source[..Cursor];
    /// <inheritdoc/>
    public string Remaining => Source[Cursor..];
    /// <inheritdoc/>
    public bool CanRead => CanReadLength(1);
    /// <inheritdoc/>
    public char Peek => PeekAt(0);

    /// <summary>Creates a reader with the same source and cursor as another reader.</summary>
    /// <param name="reader">The reader state to copy.</param>
    public StringReader(StringReader reader) : this(reader.Source, reader.Cursor)
    {
        // Empty
    }

    /// <inheritdoc/>
    public bool CanReadLength(int length)
    {
        return Cursor + length <= TotalLength;
    }

    /// <inheritdoc/>
    public char PeekAt(int offset)
    {
        return Source[Cursor + offset];
    }

    /// <summary>Returns the current character and advances the cursor by one.</summary>
    /// <returns>The consumed character.</returns>
    public char ReadNext()
    {
        return Source[Cursor++];
    }

    /// <summary>Advances the cursor by one without reading the character.</summary>
    public void Skip()
    {
        Cursor++;
    }

    /// <summary>Advances past consecutive Unicode whitespace characters.</summary>
    public void SkipWhitespace()
    {
        while (CanRead && char.IsWhiteSpace(Peek))
            Skip();
    }

    /// <summary>Reads a quoted or unquoted Boolean token.</summary>
    /// <returns>The parsed Boolean.</returns>
    /// <exception cref="CommandSyntaxException">No token is present or the token is not <c>true</c> or <c>false</c>.</exception>
    public bool ReadBoolean()
    {
        var start = Cursor;
        var value = ReadString();

        if (value.Length is 0)
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedBool.CreateWithContext(this);

        if (bool.TryParse(value, out var result))
            return result;

        Cursor = start;
        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidBool.CreateWithContext(this, value);
    }

    /// <summary>Reads a signed 32-bit integer token using the current culture.</summary>
    /// <returns>The parsed integer.</returns>
    /// <exception cref="CommandSyntaxException">No numeric token is present or it is not a valid integer.</exception>
    public int ReadInt()
    {
        var start = Cursor;

        while (CanRead && IsAllowedNumber(Peek))
            Skip();

        var span = Source.AsSpan(start, Cursor - start);

        if (span.Length is 0)
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedInt.CreateWithContext(this);

        if (int.TryParse(span, out var result))
            return result;

        Cursor = start;
        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidInt.CreateWithContext(this, Cursor);
    }

    /// <summary>Reads a signed 64-bit integer token using the current culture.</summary>
    /// <returns>The parsed long integer.</returns>
    /// <exception cref="CommandSyntaxException">No numeric token is present or it is not a valid long integer.</exception>
    public long ReadLong()
    {
        var start = Cursor;

        while (CanRead && IsAllowedNumber(Peek))
            Skip();

        var span = Source.AsSpan(start, Cursor - start);

        if (span.Length is 0)
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedLong.CreateWithContext(this);

        if (long.TryParse(span, out var result))
            return result;

        Cursor = start;
        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidLong.CreateWithContext(this, Cursor);
    }

    /// <summary>Reads a double-precision token using the current culture.</summary>
    /// <returns>The parsed value.</returns>
    /// <exception cref="CommandSyntaxException">No numeric token is present or it is not a valid double.</exception>
    public double ReadDouble()
    {
        var start = Cursor;

        while (CanRead && IsAllowedNumber(Peek))
            Skip();

        var number = Source[start..Cursor];

        if (number.Length is 0)
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedDouble.CreateWithContext(this);

        if (double.TryParse(number, out var result))
            return result;

        Cursor = start;
        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidDouble.CreateWithContext(this, Cursor);
    }

    /// <summary>Reads a single-precision token using the current culture.</summary>
    /// <returns>The parsed value.</returns>
    /// <exception cref="CommandSyntaxException">No numeric token is present or it is not a valid float.</exception>
    public float ReadFloat()
    {
        var start = Cursor;

        while (CanRead && IsAllowedNumber(Peek))
            Skip();

        var number = Source[start..Cursor];

        if (number.Length is 0)
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedFloat.CreateWithContext(this);

        if (float.TryParse(number, out var result))
            return result;

        Cursor = start;
        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat.CreateWithContext(this, Cursor);
    }

    /// <summary>Reads consecutive characters accepted by <see cref="IsAllowedInUnquotedString"/>.</summary>
    /// <returns>The consumed string, which may be empty.</returns>
    public string ReadUnquotedString()
    {
        var start = Cursor;

        while (CanRead && IsAllowedInUnquotedString(Peek))
            Skip();

        return Source[start..Cursor];
    }

    /// <summary>Reads a single- or double-quoted string including escape processing.</summary>
    /// <returns>The unescaped contents, or an empty string at end of input.</returns>
    /// <exception cref="CommandSyntaxException">The next character is not a quote, an escape is invalid, or the quote is unclosed.</exception>
    public string ReadQuotedString()
    {
        if (!CanRead)
            return string.Empty;

        var next = Peek;

        if (!IsQuotedStringStart(next))
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedStartOfQuote.CreateWithContext(this);

        Skip();

        return ReadStringUntil(next);
    }

    /// <summary>Reads either a quoted string or an unquoted token according to the next character.</summary>
    /// <returns>The parsed string, or an empty string at end of input.</returns>
    /// <exception cref="CommandSyntaxException">A quoted token contains an invalid escape or is unclosed.</exception>
    public string ReadString()
    {
        if (!CanRead)
            return string.Empty;

        var next = Peek;

        if (IsQuotedStringStart(next))
        {
            Skip();
            return ReadStringUntil(next);
        }

        return ReadUnquotedString();
    }

    /// <summary>Reads through an unescaped terminator and processes backslash escapes.</summary>
    /// <param name="terminator">The quote character that ends the string.</param>
    /// <returns>The unescaped contents without the terminator.</returns>
    /// <exception cref="CommandSyntaxException">An escape targets an unsupported character or the terminator is absent.</exception>
    public string ReadStringUntil(char terminator)
    {
        var builder = new StringBuilder();
        var escaped = false;

        while (CanRead)
        {
            var character = ReadNext();

            if (escaped)
            {
                if (character == terminator || character is SyntaxEscape)
                {
                    builder.Append(character);
                    escaped = false;
                }
                else
                {
                    Cursor--;
                    throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidEscape.CreateWithContext(this, character);
                }
            }
            else if (character == SyntaxEscape)
            {
                escaped = true;
            }
            else if (character == terminator)
            {
                return builder.ToString();
            }
            else
            {
                builder.Append(character);
            }
        }

        throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedEndOfQuote.CreateWithContext(this);
    }

    /// <summary>Consumes an expected character.</summary>
    /// <param name="character">The required next character.</param>
    /// <exception cref="CommandSyntaxException">The source ended or the next character differs.</exception>
    public void Expect(char character)
    {
        if (!CanRead || Peek != character)
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol.CreateWithContext(this, character);

        Skip();
    }

    /// <summary>Determines whether a character may be consumed as part of a numeric token.</summary>
    /// <param name="digit">The character to test.</param>
    /// <returns><see langword="true"/> for an ASCII digit, period, or hyphen.</returns>
    public static bool IsAllowedNumber(char digit)
    {
        return digit is >= '0' and <= '9' or '.' or '-';
    }

    /// <summary>Determines whether a character opens a quoted string.</summary>
    /// <param name="character">The character to test.</param>
    /// <returns><see langword="true"/> for a single or double quote.</returns>
    public static bool IsQuotedStringStart(char character)
    {
        return character is SyntaxDoubleQuote or SyntaxSingleQuote;
    }

    /// <summary>Determines whether a character is accepted in an unquoted argument token.</summary>
    /// <param name="character">The character to test.</param>
    /// <returns><see langword="true"/> for an ASCII letter, digit, underscore, hyphen, period, or plus.</returns>
    public static bool IsAllowedInUnquotedString(char character)
    {
        return character is
            >= '0' and <= '9'
            or >= 'A' and <= 'Z'
            or >= 'a' and <= 'z'
            or '_' or '-'
            or '.' or '+';
    }
}
