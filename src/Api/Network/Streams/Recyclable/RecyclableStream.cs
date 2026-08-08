using Microsoft.IO;

namespace Void.Proxy.Api.Network.Streams.Recyclable;

/// <summary>
/// Provides the shared recyclable-memory-stream pool used by proxy stream implementations.
/// </summary>
public abstract class RecyclableStream
{
    /// <summary>
    /// The shared manager used to rent recyclable streams and buffers.
    /// </summary>
    public static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new(new RecyclableMemoryStreamManager.Options
    {
        BlockSize = 1024,
        LargeBufferMultiple = 1024 * 1024,
        MaximumBufferSize = 16 * 1024 * 1024,
        GenerateCallStacks = false,
        AggressiveBufferReturn = true,
        MaximumLargePoolFreeBytes = 16 * 1024 * 1024,
        MaximumSmallPoolFreeBytes = 100 * 1024
    });
}
