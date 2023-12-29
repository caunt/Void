using System.Buffers;

namespace Void.Proxy.API.Network.IO;

public readonly struct MinecraftMessage(Memory<byte> memory, IMemoryOwner<byte> owner) : IDisposable
{
    public Memory<byte> Memory { get; } = memory;
    public void Dispose() => owner.Dispose();
}