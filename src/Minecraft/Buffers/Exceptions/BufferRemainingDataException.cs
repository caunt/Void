using System;

namespace Void.Minecraft.Buffers.Exceptions;

/// <summary>
/// Represents an error that occurs when a buffer operation completes with unread bytes remaining.
/// </summary>
/// <param name="bufferSize">
/// The total length of the buffer that was being consumed.
/// </param>
/// <param name="bufferPosition">
/// The final read position reached when the operation ended.
/// </param>
/// <remarks>
/// This exception is thrown by <see cref="Void.Minecraft.Buffers.BufferSpan.Dispose"/> when the span is disposed before all bytes are consumed.
/// </remarks>
public class BufferRemainingDataException(long bufferSize, long bufferPosition) : Exception($"Buffer has remaining {bufferSize - bufferPosition} bytes of data (read {bufferPosition}/{bufferSize}).")
{
    /// <summary>
    /// Gets the total size of the buffer.
    /// </summary>
    /// <value>
    /// The number of bytes available in the source buffer.
    /// </value>
    public long BufferSize { get; } = bufferSize;

    /// <summary>
    /// Gets the read position at the time the exception was created.
    /// </summary>
    /// <value>
    /// The number of bytes that had been consumed from the buffer.
    /// </value>
    public long BufferPosition { get; } = bufferPosition;

    /// <summary>
    /// Gets the number of unread bytes remaining in the buffer.
    /// </summary>
    /// <value>
    /// The difference between <see cref="BufferSize"/> and <see cref="BufferPosition"/>.
    /// </value>
    public long RemainingData { get; } = bufferSize - bufferPosition;
}
