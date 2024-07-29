using System.Buffers;

namespace Void.Proxy.API.Network.IO.Messages;

public class BufferedBinaryMessage(
    Memory<byte> memory,
    IMemoryOwner<byte> owner) : IMinecraftMessage
{
    public Memory<byte> Memory => memory;

    public void Dispose()
    {
        owner.Dispose();
    }
}