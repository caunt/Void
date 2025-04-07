using Microsoft.IO;

namespace Void.Proxy.Api.Network.Streams.Recyclable;

public abstract class RecyclableStream
{
    public static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new(new RecyclableMemoryStreamManager.Options
    {
        // TODO: replace BlockSize to 1024, but that will cause some packets to be unable to read
        BlockSize = 2048,
        LargeBufferMultiple = 1024 * 1024,
        MaximumBufferSize = 16 * 1024 * 1024,
        GenerateCallStacks = false,
        AggressiveBufferReturn = true,
        MaximumLargePoolFreeBytes = 16 * 1024 * 1024,
        MaximumSmallPoolFreeBytes = 100 * 1024
    });
}
