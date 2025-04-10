using System;
using System.Buffers.Binary;

namespace Void.Minecraft.Buffers.Extensions;

public static class ReadMinecraftBufferExtensions
{
    /// <summary>
    /// Reads a single byte from the provided buffer.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter is a reference to a buffer from which a byte will be read.</param>
    /// <returns>Returns the byte read from the buffer.</returns>
    public static byte ReadByte<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.Read(1)[0];

    /// <summary>
    /// Reads a 16-bit unsigned integer from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the unsigned short is read.</param>
    /// <returns>Returns the 16-bit unsigned integer read from the buffer.</returns>
    public static ushort ReadUnsignedShort<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadUInt16BigEndian(buffer.Read(2));

    /// <summary>
    /// Reads a 16-bit short value from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the short value is read.</param>
    /// <returns>Returns the 16-bit short value read from the buffer.</returns>
    public static short ReadShort<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadInt16BigEndian(buffer.Read(2));

    /// <summary>
    /// Reads a 32-bit integer from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type used for the buffer must be a struct that implements a specific interface.</typeparam>
    /// <param name="buffer">The buffer from which the integer is read, passed by reference.</param>
    /// <returns>The 32-bit integer read from the buffer.</returns>
    public static int ReadInt<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadInt32BigEndian(buffer.Read(4));

    /// <summary>
    /// Reads a 4-byte floating-point value from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer from which the floating-point value is read.</param>
    /// <returns>The method returns the floating-point value read from the buffer.</returns>
    public static float ReadFloat<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadSingleBigEndian(buffer.Read(4));

    /// <summary>
    /// Reads an 8-byte double value from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the double value is read.</param>
    /// <returns>The method returns the double value read from the buffer.</returns>
    public static double ReadDouble<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadDoubleBigEndian(buffer.Read(8));

    /// <summary>
    /// Reads a 64-bit integer from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the integer is read.</param>
    /// <returns>Returns the 64-bit integer read from the buffer.</returns>
    public static long ReadLong<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadInt64BigEndian(buffer.Read(8));

    /// <summary>
    /// Reads a specified number of bytes from a buffer and returns them as a read-only span.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The buffer from which bytes are read and manipulated.</param>
    /// <param name="length">The number of bytes to read from the buffer.</param>
    /// <returns>A read-only span containing the bytes that were read from the buffer.</returns>
    public static ReadOnlySpan<byte> Read<TBuffer>(ref this TBuffer buffer, int length) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var slice = buffer.Access(length);
        buffer.Seek(length);
        return slice;
    }
}
