using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Memory;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.API.Network.IO.Messages;

public class BinaryPacket(int id, MemoryHolder holder) : IMinecraftPacket
{
    public int Id => id;
    public MemoryHolder Holder => holder;

    public void Dispose()
    {
        holder.Dispose();
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