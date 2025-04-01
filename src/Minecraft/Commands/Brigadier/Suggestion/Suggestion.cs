using System;
using System.Text;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

public record Suggestion(StringRange Range, string Text, IMessage? Tooltip = null) : IComparable<Suggestion>
{
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

    public int CompareTo(Suggestion? other)
    {
        return string.Compare(Text, other?.Text, StringComparison.Ordinal);
    }

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
