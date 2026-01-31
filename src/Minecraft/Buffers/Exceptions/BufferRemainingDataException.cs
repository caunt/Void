using System;

namespace Void.Minecraft.Buffers.Exceptions;

public class BufferRemainingDataException(long bufferSize, long bufferPosition) : Exception($"Buffer has remaining {bufferSize - bufferPosition} bytes of data (read {bufferPosition}/{bufferSize}).")
{
    public long BufferSize { get; } = bufferSize;
    public long BufferPosition { get; } = bufferPosition;
    public long RemainingData { get; } = bufferSize - bufferPosition;
}
