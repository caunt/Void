using System;
using System.Buffers.Binary;

namespace Void.Minecraft.Buffers.Extensions;

public static class WriteMinecraftBufferExtensions
{
    /// <summary>
    /// Writes a single byte value to a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The buffer is modified to store the byte value at a designated position.</param>
    /// <param name="value">The byte value to be written into the buffer.</param>
    public static void WriteByte<TBuffer>(ref this TBuffer buffer, byte value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        buffer.AccessWrite(1)[0] = value;
    }

    /// <summary>
    /// Writes an unsigned short value to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for writing operations.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the unsigned short value will be written.</param>
    /// <param name="value">This parameter is the unsigned short value that will be written to the buffer.</param>
    public static void WriteUnsignedShort<TBuffer>(ref this TBuffer buffer, ushort value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer.AccessWrite(2), value);
    }

    /// <summary>
    /// Writes a short integer to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the short integer will be written.</param>
    /// <param name="value">This parameter is the short integer value to be written to the buffer.</param>
    public static void WriteShort<TBuffer>(ref this TBuffer buffer, short value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer.AccessWrite(2), value);
    }

    /// <summary>
    /// Writes a 32-bit integer to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a struct that implements a specific buffer interface, allowing for efficient memory access.</typeparam>
    /// <param name="buffer">The buffer is modified to include the new integer value at the appropriate position.</param>
    /// <param name="value">The integer to be written into the buffer in big-endian format.</param>
    public static void WriteInt<TBuffer>(ref this TBuffer buffer, int value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        BinaryPrimitives.WriteInt32BigEndian(buffer.AccessWrite(4), value);
    }

    /// <summary>
    /// Writes a floating-point number in big-endian format to a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a structure that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">The buffer is modified to store the floating-point value in a specific format.</param>
    /// <param name="value">The floating-point number to be written into the buffer.</param>
    public static void WriteFloat<TBuffer>(ref this TBuffer buffer, float value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        BinaryPrimitives.WriteSingleBigEndian(buffer.AccessWrite(4), value);
    }

    /// <summary>
    /// Writes a double value to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the double value will be written.</param>
    /// <param name="value">This parameter is the double value that will be written to the buffer.</param>
    public static void WriteDouble<TBuffer>(ref this TBuffer buffer, double value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        BinaryPrimitives.WriteDoubleBigEndian(buffer.AccessWrite(8), value);
    }

    /// <summary>
    /// Writes a 64-bit integer to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a struct that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">The buffer is modified to accommodate the new data being written.</param>
    /// <param name="value">The 64-bit integer to be written into the buffer.</param>
    public static void WriteLong<TBuffer>(ref this TBuffer buffer, long value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        BinaryPrimitives.WriteInt64BigEndian(buffer.AccessWrite(8), value);
    }

    /// <summary>
    /// Writes a byte span to a buffer, allowing for efficient data handling in a structured format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a struct that implements a specific buffer interface for Minecraft data
    /// operations.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer that will receive the byte data.</param>
    /// <param name="data">This parameter contains the byte data that will be copied into the buffer.</param>
    public static void Write<TBuffer>(ref this TBuffer buffer, scoped ReadOnlySpan<byte> data) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        data.CopyTo(buffer.AccessWrite(data.Length));
    }

    private static Span<byte> AccessWrite<TBuffer>(ref this TBuffer buffer, int length) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var span = buffer.Access(length);
        buffer.Seek(length);
        return span;
    }
}
