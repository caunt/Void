using Microsoft.IO;

namespace Void.Proxy.Api.Network.Streams.Recyclable;

public abstract class RecyclableStream
{
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
