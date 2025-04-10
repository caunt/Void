using System;
using System.IO;

namespace Void.Minecraft.Buffers;

public interface ICommonMinecraftBuffer
{
    /// <summary>
    /// Gets or sets the position within a buffer. The position must be between 0 and the length of the source,
    /// otherwise an exception is thrown.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Returns the length of the source as an integer. It is a read-only property.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Advances or sets the current position in the buffer relative to the specified origin.
    /// </summary>
    /// <param name="offset">The byte offset relative to the <paramref name="origin"/>.</param>
    /// <param name="origin">The point of reference used to obtain the new position.</param>
    public void Seek(int offset, SeekOrigin origin = SeekOrigin.Current);

    /// <summary>
    /// Retrieves a span of bytes from the underlying buffer starting at the current position
    /// for the specified length. This method does not modify the current position of the buffer.
    /// </summary>
    /// <param name="length">The number of bytes to retrieve from the current position. Must be non-negative.</param>
    /// <returns>
    /// A <see cref="Span{Byte}"/> representing the specified number of bytes from the current position.
    /// </returns>
    public Span<byte> Access(int length);

    /// <summary>
    /// Retrieves a span of bytes from the underlying buffer starting at the specified position
    /// for the given length.
    /// </summary>
    /// <param name="position">
    /// The starting position in the buffer from which to retrieve the byte span. Must be a non-negative value.
    /// </param>
    /// <param name="length">
    /// The number of bytes to include in the retrieved span. Must be non-negative.
    /// </param>
    /// <returns>
    /// A <see cref="Span{Byte}"/> containing the specified range of bytes.
    /// </returns>
    public Span<byte> Access(int position, int length);
}

public interface IReadMinecraftBuffer : ICommonMinecraftBuffer
{
    /// <summary>
    /// Calculates the number of remaining elements in a source collection. It subtracts the current position from the
    /// total length.
    /// </summary>
    public int Remaining { get; }
}

public interface IWriteMinecraftBuffer : ICommonMinecraftBuffer;

public interface IMinecraftBuffer<TBuffer> : IReadMinecraftBuffer, IWriteMinecraftBuffer where TBuffer : struct, allows ref struct
{
    /// <summary>
    /// Extracts a portion of a buffer starting from a specified position for a given length.
    /// </summary>
    /// <param name="position">Indicates the starting point from which the extraction begins.</param>
    /// <param name="length">Specifies the number of elements to include in the extracted portion.</param>
    /// <returns>Returns a new buffer containing the specified slice of the original buffer.</returns>
    public TBuffer Slice(int position, int length);
}
