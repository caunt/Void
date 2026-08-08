using System;
using System.Text;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

/// <summary>Describes text that replaces an input range, with an optional tooltip.</summary>
/// <param name="Range">The half-open range replaced by the suggestion.</param>
/// <param name="Text">The replacement text.</param>
/// <param name="Tooltip">The optional display tooltip.</param>
public record Suggestion(StringRange Range, string Text, IMessage? Tooltip = null) : IComparable<Suggestion>
{
    /// <summary>Applies this replacement to a source string.</summary>
    /// <param name="source">The complete input.</param>
    /// <returns>The input with <see cref="Range"/> replaced by <see cref="Text"/>.</returns>
    public string Apply(string source)
    {
        if (Range.Start == 0 && Range.End == source.Length)
            return Text;

        var span = source.AsSpan();
        var builder = new StringBuilder();

        if (Range.Start > 0)
            builder.Append(span[..Range.Start]);

        builder.Append(Text);

        if (Range.End < source.Length)
            builder.Append(span[Range.End..]);

        return builder.ToString();
    }

    /// <summary>Compares suggestion text using ordinal ordering.</summary>
    /// <param name="other">The other suggestion, whose absent text compares as <see langword="null"/>.</param>
    /// <returns>The ordinal comparison result.</returns>
    public int CompareTo(Suggestion? other)
    {
        return string.Compare(Text, other?.Text, StringComparison.Ordinal);
    }

    /// <summary>Expands this suggestion to cover a containing range by retaining intervening source text.</summary>
    /// <param name="source">The complete input.</param>
    /// <param name="range">The new replacement range.</param>
    /// <returns>This instance when ranges match; otherwise an expanded suggestion.</returns>
    public Suggestion Expand(string source, StringRange range)
    {
        if (range == Range)
            return this;

        var span = source.AsSpan();
        var builder = new StringBuilder();

        if (range.Start < Range.Start)
            builder.Append(span[range.Start..Range.Start]);

        builder.Append(Text);

        if (range.End > Range.End)
            builder.Append(span[Range.End..range.End]);

        return new Suggestion(range, builder.ToString(), Tooltip);
    }
}
