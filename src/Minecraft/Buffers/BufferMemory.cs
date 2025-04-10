using System;
using Void.Minecraft.Buffers.Exceptions;

namespace Void.Minecraft.Buffers;

/// <summary>
/// Represents a memory buffer that can be sliced into smaller segments. Provides a way to access a specific range of
/// bytes within the buffer.
/// </summary>
/// <param name="source">The input memory provides the raw byte data to be manipulated and accessed.</param>
public readonly struct BufferMemory(Memory<byte> source)
{
    /// <summary>
    /// Returns a new BufferSpan instance based on the source's Span. It provides a read-only view of the underlying
    /// buffer.
    /// </summary>
    public readonly BufferSpan Span => new(source.Span);

    /// <summary>
    /// Creates a new BufferMemory instance that represents a portion of the source buffer.
    /// </summary>
    /// <param name="position">Specifies the starting index of the slice within the source buffer.</param>
    /// <param name="length">Indicates the number of elements to include in the slice from the starting index.</param>
    /// <returns>Returns a BufferMemory object containing the specified slice of the source buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the starting index or length is negative.</exception>
    /// <exception cref="EndOfBufferException">Thrown when the sum of the starting index and length exceeds the source buffer's length.</exception>
    public readonly BufferMemory Slice(int position, int length)
    {
        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(position), "Value must be non-negative.");

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Value must be non-negative.");

        if (position + length > source.Length)
            throw new EndOfBufferException(position, length, source.Length);

        return new BufferMemory(source.Slice(position, length));
    }
}
