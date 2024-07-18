using System.Buffers;

namespace Void.Proxy.API.Network.IO.Messages;

public readonly struct CompleteBinaryMessage(
    Memory<byte> memory,
    IMemoryOwner<byte> owner) : IMinecraftMessage, IDisposable
{
    public Memory<byte> Memory => memory;

    public void Dispose()
    {
        owner.Dispose();
    }
}