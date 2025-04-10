using System;
using System.Numerics;

namespace Void.Minecraft.Buffers.Extensions;

public static class IntExtensions
{
    public static int VarIntSize(this int value) => (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;

    public static Span<byte> AsVarInt(this int value)
    {
        Span<byte> buffer = stackalloc byte[5];

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

        return buffer[..index].ToArray();
    }
}
