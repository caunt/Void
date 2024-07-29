using System.Buffers;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.API.Network.IO.Messages;

public class BinaryPacket(int id, Memory<byte> memory, IMemoryOwner<byte> owner) : IMinecraftPacket
{
    public int Id => id;
    public Memory<byte> Memory => memory;

    public void Dispose()
    {
        owner.Dispose();
    }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        throw new InvalidOperationException();
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        throw new InvalidOperationException();
    }
}