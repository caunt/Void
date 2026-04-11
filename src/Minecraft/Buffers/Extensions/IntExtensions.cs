using System;
using System.Collections.Generic;
using System.Numerics;

namespace Void.Minecraft.Buffers.Extensions;

public static class IntExtensions
{
    /// <summary>
    /// Computes the number of bytes required to encode <paramref name="value"/> as a Minecraft VarInt.
    /// </summary>
    /// <param name="value">The 32-bit signed integer to evaluate.</param>
    /// <returns>
    /// The encoded VarInt length in bytes, always between <c>1</c> and <c>5</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs a branch-free bit operation and does not allocate.
    /// </para>
    /// <para>
    /// The result matches the byte count written by <see cref="AsVarInt(int, Span{byte})" /> for the same value,
    /// which allows callers to precompute packet offsets and payload lengths.
    /// </para>
    /// <para>
    /// Because Minecraft VarInt uses the two's-complement bit pattern of signed integers, negative values always
    /// require <c>5</c> bytes.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var idLength = packetId.VarIntSize();
    /// binaryMessage.Stream.Position = idLength;
    /// </code>
    /// </example>
    /// <seealso cref="AsVarInt(int)" />
    /// <seealso cref="AsVarInt(int, Span{byte})" />
    public static int VarIntSize(this int value) => (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;

    /// <summary>
    /// Encodes <paramref name="value"/> into a new byte array using the Minecraft VarInt format.
    /// </summary>
    /// <param name="value">The 32-bit signed value to encode.</param>
    /// <returns>
    /// A new array containing only the encoded VarInt bytes (length from <c>1</c> to <c>5</c>).
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method allocates a new array for every call and copies the encoded payload from an internal
    /// stack buffer.
    /// </para>
    /// <para>
    /// Negative values are encoded from their two's-complement bit pattern and therefore produce <c>5</c> bytes.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// byte[] payload = 300.AsVarInt();
    /// // payload can be used as a compact packet field representation.
    /// </code>
    /// </example>
    /// <see cref="AsVarInt(int, Span&lt;byte&gt;)" />
    /// <seealso cref="VarIntSize(int)" />
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

    /// <summary>
    /// Lazily enumerates bytes of <paramref name="value"/> encoded as a Minecraft VarInt.
    /// </summary>
    /// <param name="value">The 32-bit signed integer to encode.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that yields encoded bytes in wire order, from first to last byte.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This API is obsolete; prefer <see cref="AsVarInt(int)" /> or <see cref="AsVarInt(int, Span{byte})" /> for
    /// clearer ownership and lower overhead.
    /// </para>
    /// <para>
    /// Enumeration executes the encoding loop on demand each time the sequence is iterated. The sequence length is
    /// from <c>1</c> to <c>5</c> bytes, and negative values produce <c>5</c> bytes.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// foreach (var b in IntExtensions.EnumerateVarInt(300))
    /// {
    ///     // consume each encoded VarInt byte
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="VarIntSize(int)" />
    /// <seealso cref="AsVarInt(int)" />
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
