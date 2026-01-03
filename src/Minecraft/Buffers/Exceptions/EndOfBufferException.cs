using System;

namespace Void.Minecraft.Buffers.Exceptions;

/// <summary>
/// Indicates an attempt to access data beyond the available buffer size.
/// </summary>
/// <param name="position">Specifies the starting point of the attempted access within the buffer.</param>
/// <param name="length">Indicates the number of bytes that were attempted to be accessed.</param>
/// <param name="size">Represents the total size of the buffer available for access.</param>
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
