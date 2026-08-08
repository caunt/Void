using System;

namespace Void.Minecraft.Commands.Brigadier.Context;

/// <summary>Represents a half-open character range in command input.</summary>
/// <param name="Start">The inclusive start index.</param>
/// <param name="End">The exclusive end index.</param>
public record StringRange(int Start, int End)
{
    /// <summary>Gets whether the range has zero length.</summary>
    public bool IsEmpty => Start == End;
    /// <summary>Gets <see cref="End"/> minus <see cref="Start"/>.</summary>
    public int Length => End - Start;

    /// <summary>Creates an empty range at a position.</summary>
    /// <param name="pos">The start and end position.</param>
    /// <returns>The empty range.</returns>
    public static StringRange At(int pos)
    {
        return new StringRange(pos, pos);
    }

    /// <summary>Creates a range between two supplied indexes without validation.</summary>
    /// <param name="start">The inclusive start.</param>
    /// <param name="end">The exclusive end.</param>
    /// <returns>The range.</returns>
    public static StringRange Between(int start, int end)
    {
        return new StringRange(start, end);
    }

    /// <summary>Creates the smallest range containing two ranges.</summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns>A range from the lesser start through the greater end.</returns>
    public static StringRange Encompassing(StringRange a, StringRange b)
    {
        return new StringRange(Math.Min(a.Start, b.Start), Math.Max(a.End, b.End));
    }

    /// <summary>Extracts this range from a reader's source.</summary>
    /// <param name="reader">The source provider.</param>
    /// <returns>The selected substring.</returns>
    public string Get(IImmutableStringReader reader)
    {
        return reader.Source[Start..End];
    }

    /// <summary>Extracts this range from a string.</summary>
    /// <param name="value">The source string.</param>
    /// <returns>The selected substring.</returns>
    public string Get(string value)
    {
        return value[Start..End];
    }
}
