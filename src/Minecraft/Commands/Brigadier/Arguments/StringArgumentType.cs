using System;
using System.Collections.Generic;

namespace Void.Minecraft.Commands.Brigadier.Arguments;

public record StringArgumentType : IArgumentType<string>
{
    public IEnumerable<string> Examples => Type switch
    {
        StringType.SingleWord => ["word", "words_with_underscores"],
        StringType.QuotablePhrase => ["\"quoted phrase\"", "word", "\"\""],
        StringType.GreedyPhrase => ["word", "words with spaces", "\"and symbols\""],
        _ => throw new ArgumentOutOfRangeException(nameof(Type)),
    };

    public StringType Type { get; init; }

    private StringArgumentType()
    {
        // Empty
    }

    public static StringArgumentType Word()
    {
        return new StringArgumentType { Type = StringType.SingleWord };
    }

    public static StringArgumentType String()
    {
        return new StringArgumentType { Type = StringType.QuotablePhrase };
    }

    public static StringArgumentType GreedyString()
    {
        return new StringArgumentType { Type = StringType.GreedyPhrase };
    }

    public string Parse(StringReader reader)
    {
        if (Type == StringType.GreedyPhrase)
        {
            var text = reader.Remaining;
            reader.Cursor = reader.TotalLength;
            return text;
        }
        else if (Type == StringType.SingleWord)
        {
            return reader.ReadUnquotedString();
        }
        else
        {
            return reader.ReadString();
        }
    }

    public enum StringType
    {
        SingleWord,
        QuotablePhrase,
        GreedyPhrase,
    }
}
