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
    public long BufferSize { get; } = bufferSize;
    public long BufferPosition { get; } = bufferPosition;
    public long RequestedLength { get; } = requestedLength;

    public EndOfBufferException(int bufferSize, int bufferPosition, int requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }

    public EndOfBufferException(nint bufferSize, nint bufferPosition, nint requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }

    public EndOfBufferException(short bufferSize, short bufferPosition, short requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }

    public EndOfBufferException(byte bufferSize, byte bufferPosition, byte requestedLength) : this((long)bufferSize, bufferPosition, requestedLength)
    {
        // Intentionally left blank
    }
}
