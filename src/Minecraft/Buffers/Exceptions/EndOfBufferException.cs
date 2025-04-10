using System;

namespace Void.Minecraft.Buffers.Exceptions;

/// <summary>
/// Indicates an attempt to access data beyond the available buffer size.
/// </summary>
/// <param name="position">Specifies the starting point of the attempted access within the buffer.</param>
/// <param name="length">Indicates the number of bytes that were attempted to be accessed.</param>
/// <param name="size">Represents the total size of the buffer available for access.</param>
public class EndOfBufferException(int position, int length, int size) : Exception($"You tried to access {length} bytes from the buffer, but only {size - position} bytes are available ({position}/{size}).")
{
    public int Position { get; } = position;
    public int Length { get; } = length;
    public int Size { get; } = size;
}
