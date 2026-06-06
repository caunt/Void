using System;

namespace Void.Minecraft.Buffers.Exceptions;

/// <summary>
/// Indicates an attempt to access data beyond the available buffer size.
/// </summary>
/// <param name="bufferSize">The total size of the buffer in bytes.</param>
/// <param name="bufferPosition">The current read position within the buffer at the time of the failed access.</param>
/// <param name="requestedLength">The number of bytes that were requested but could not be satisfied.</param>
public class EndOfBufferException(long bufferSize, long bufferPosition, long requestedLength) : Exception($"You tried to access {requestedLength} bytes from the buffer, but only {bufferSize - bufferPosition} bytes are available ({bufferPosition}/{bufferSize}).")
{
    /// <summary>
    /// The size of the buffer in bytes.
    /// </summary>
    public long BufferSize { get; } = bufferSize;

    /// <summary>
    /// The position within the buffer.
    /// </summary>
    public long BufferPosition { get; } = bufferPosition;

    /// <summary>
    /// Gets the requested length.
    /// </summary>
    public long RequestedLength { get; } = requestedLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="EndOfBufferException"/> class with the specified buffer size,
    /// position, and requested length.
    /// </summary>
    /// <param name="bufferSize">The size of the buffer.</param>
    /// <param name="bufferPosition">The position in the buffer where the read operation was attempted.</param>
    /// <param name="requestedLength">The length of data requested.</param>
    public EndOfBufferException(int bufferSize, int bufferPosition, int requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndOfBufferException"/> class with the specified buffer size,
    /// position, and requested length.
    /// </summary>
    /// <param name="bufferSize">The size of the buffer.</param>
    /// <param name="bufferPosition">The current position in the buffer.</param>
    /// <param name="requestedLength">The length of data that was requested.</param>
    public EndOfBufferException(nint bufferSize, nint bufferPosition, nint requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndOfBufferException"/> class with 16-bit buffer information.
    /// </summary>
    /// <param name="bufferSize">The total size of the buffer.</param>
    /// <param name="bufferPosition">The current position in the buffer where the read was attempted.</param>
    /// <param name="requestedLength">The number of bytes that were requested.</param>
    public EndOfBufferException(short bufferSize, short bufferPosition, short requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndOfBufferException"/> class with byte-sized buffer metrics.
    /// </summary>
    /// <param name="bufferSize">The size of the buffer.</param>
    /// <param name="bufferPosition">The current position in the buffer.</param>
    /// <param name="requestedLength">The requested length that would exceed the buffer bounds.</param>
    public EndOfBufferException(byte bufferSize, byte bufferPosition, byte requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }
}
