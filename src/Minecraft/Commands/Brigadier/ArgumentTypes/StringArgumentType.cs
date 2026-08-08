using System;
using System.Collections.Generic;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

/// <summary>Contains a parsed string argument.</summary>
/// <param name="Value">The parsed string.</param>
public record StringArgumentValue(string Value) : IArgumentValue;
/// <summary>Parses one word, a quotable phrase, or the entire remaining input.</summary>
public record StringArgumentType : IArgumentType
{
    /// <inheritdoc/>
    public IEnumerable<string> Examples => Type switch
    {
        StringType.SingleWord => ["word", "words_with_underscores"],
        StringType.QuotablePhrase => ["\"quoted phrase\"", "word", "\"\""],
        StringType.GreedyPhrase => ["word", "words with spaces", "\"and symbols\""],
        _ => throw new ArgumentOutOfRangeException(nameof(Type)),
    };

    /// <summary>Gets the string parsing mode.</summary>
    public StringType Type { get; init; }

    private StringArgumentType()
    {
        // Empty
    }

    /// <summary>Creates a single-unquoted-word argument.</summary>
    /// <returns>The argument type.</returns>
    public static StringArgumentType Word()
    {
        return new StringArgumentType { Type = StringType.SingleWord };
    }

    /// <summary>Creates an argument accepting either a quoted phrase or one unquoted word.</summary>
    /// <returns>The argument type.</returns>
    public static StringArgumentType String()
    {
        return new StringArgumentType { Type = StringType.QuotablePhrase };
    }

    /// <summary>Creates an argument that consumes every remaining character.</summary>
    /// <returns>The argument type.</returns>
    public static StringArgumentType GreedyString()
    {
        return new StringArgumentType { Type = StringType.GreedyPhrase };
    }

    /// <inheritdoc/>
    public IArgumentValue Parse(StringReader reader)
    {
        if (Type == StringType.GreedyPhrase)
        {
            var text = reader.Remaining;
            reader.Cursor = reader.TotalLength;
            return new StringArgumentValue(text);
        }
        else if (Type == StringType.SingleWord)
        {
            return new StringArgumentValue(reader.ReadUnquotedString());
        }
        else
        {
            return new StringArgumentValue(reader.ReadString());
        }
    }

    /// <summary>Specifies how a string argument consumes input.</summary>
    public enum StringType
    {
        /// <summary>Consume one unquoted token.</summary>
        SingleWord,
        /// <summary>Consume a quoted phrase or one unquoted token.</summary>
        QuotablePhrase,
        /// <summary>Consume the complete remaining input without unescaping it.</summary>
        GreedyPhrase,
    }
}
