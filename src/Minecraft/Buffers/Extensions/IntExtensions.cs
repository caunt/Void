using System;
using System.Collections.Generic;
using System.Numerics;

namespace Void.Minecraft.Buffers.Extensions;

public static class IntExtensions
{
    public static int VarIntSize(this int value) => (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;

    public static byte[] AsVarInt(this int value)
    {
        Span<byte> buffer = stackalloc byte[5];
        var length = value.AsVarInt(buffer);
        return [.. buffer[..length]];
    }

    public static int AsVarInt(this int value, Span<byte> buffer)
    {
        var unsigned = (uint)value;
        var index = 0;

        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            buffer[index++] = temp;
        } while (unsigned != 0);

        return index;
    }

    [Obsolete("Use AsVarInt instead.")]
    public static IEnumerable<byte> EnumerateVarInt(int value)
    {
        var unsigned = (uint)value;

        do
        {
            var temp = (byte)(unsigned & 127);

            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            yield return temp;
        } while (unsigned != 0);
    }
}
