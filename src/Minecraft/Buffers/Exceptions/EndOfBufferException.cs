using System;

namespace Void.Minecraft.Buffers.Exceptions;

/// <summary>
/// Indicates an attempt to access data beyond the available buffer size.
/// </summary>
/// <param name="position">Specifies the starting point of the attempted access within the buffer.</param>
/// <param name="length">Indicates the number of bytes that were attempted to be accessed.</param>
/// <param name="size">Represents the total size of the buffer available for access.</param>
public class EndOfBufferException(long position, long length, long size) : Exception($"You tried to access {length} bytes from the buffer, but only {size - position} bytes are available ({position}/{size}).")
{
    public long Position { get; } = position;
    public long Length { get; } = length;
    public long Size { get; } = size;

    public EndOfBufferException(int position, int length, int size) : this((long)position, length, size)
    {
        // Intentionally left blank
    }

    public EndOfBufferException(nint position, nint length, nint size) : this((long)position, length, size)
    {
        // Intentionally left blank
    }

    public EndOfBufferException(short position, short length, short size) : this((long)position, length, size)
    {
        // Intentionally left blank
    }

    public EndOfBufferException(byte position, byte length, byte size) : this((long)position, length, size)
    {
        // Intentionally left blank
    }
}
