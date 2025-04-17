using System;

namespace Void.Minecraft.Commands.Brigadier.Context;

public record StringRange(int Start, int End)
{
    public bool IsEmpty => Start == End;
    public int Length => End - Start;

    public static StringRange At(int pos)
    {
        return new StringRange(pos, pos);
    }

    public static StringRange Between(int start, int end)
    {
        return new StringRange(start, end);
    }

    public static StringRange Encompassing(StringRange a, StringRange b)
    {
        return new StringRange(Math.Min(a.Start, b.Start), Math.Max(a.End, b.End));
    }

    public string Get(IImmutableStringReader reader)
    {
        return reader.Source[Start..End];
    }

    public string Get(string value)
    {
        return value[Start..End];
    }
}
