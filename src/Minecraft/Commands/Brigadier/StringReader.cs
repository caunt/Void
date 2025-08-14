using System;
using System.Text;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier;

public class StringReader(string source, int cursor = 0) : IImmutableStringReader
{
    private const char SyntaxEscape = '\\';
    private const char SyntaxDoubleQuote = '"';
    private const char SyntaxSingleQuote = '\'';

    public string Source { get; init; } = source;
    public int Cursor { get; set; } = cursor;
    public int RemainingLength => TotalLength - Cursor;
    public int TotalLength => Source.Length;
    public string Read => Source[..Cursor];
    public string Remaining => Source[Cursor..];
    public bool CanRead => CanReadLength(1);
    public char Peek => PeekAt(0);

    public StringReader(StringReader reader) : this(reader.Source, reader.Cursor)
    {
        // Empty
    }

    public bool CanReadLength(int length)
    {
        return Cursor + length <= TotalLength;
    }

    public char PeekAt(int offset)
    {
        return Source[Cursor + offset];
    }

    public char ReadNext()
    {
        return Source[Cursor++];
    }

    public void Skip()
    {
        Cursor++;
    }

    public void SkipWhitespace()
    {
        while (CanRead && char.IsWhiteSpace(Peek))
            Skip();
    }

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

    public string ReadUnquotedString()
    {
        var start = Cursor;

        while (CanRead && IsAllowedInUnquotedString(Peek))
            Skip();

        return Source[start..Cursor];
    }

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

    public void Expect(char character)
    {
        if (!CanRead || Peek != character)
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol.CreateWithContext(this, character);

        Skip();
    }

    public static bool IsAllowedNumber(char digit)
    {
        return digit is >= '0' and <= '9' or '.' or '-';
    }

    public static bool IsQuotedStringStart(char character)
    {
        return character is SyntaxDoubleQuote or SyntaxSingleQuote;
    }

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
