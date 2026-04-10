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

    /// <summary>
    /// Encodes <paramref name="value"/> as a Minecraft VarInt into <paramref name="buffer"/>.
    /// </summary>
    /// <param name="value">The 32-bit signed value to encode.</param>
    /// <param name="buffer">
    /// The destination span that receives encoded bytes starting at index <c>0</c>.
    /// The span must be large enough for the encoded value.
    /// </param>
    /// <returns>
    /// The number of bytes written to <paramref name="buffer"/> (from <c>1</c> to <c>5</c>).
    /// </returns>
    /// <remarks>
    /// <para>
    /// Encoding uses the standard 7-bit continuation format used by Minecraft packets.
    /// Negative values are encoded from their two's-complement bit pattern and therefore use <c>5</c> bytes.
    /// </para>
    /// <para>
    /// The method writes only the returned byte count and does not clear any remaining bytes in
    /// <paramref name="buffer"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="buffer"/> is too small for the encoded value.
    /// </exception>
    /// <example>
    /// <code>
    /// Span&lt;byte&gt; bytes = stackalloc byte[5];
    /// var length = 300.AsVarInt(bytes);
    /// // bytes[..length] now contains the VarInt payload.
    /// </code>
    /// </example>
    /// <see cref="AsVarInt(int)" />
    /// <seealso cref="VarIntSize(int)" />
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
