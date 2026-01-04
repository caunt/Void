using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

public class KeepAliveRequestPacket : IMinecraftServerboundPacket<KeepAliveRequestPacket>
{
    public long Id { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteLong(Id);
    }

    public static KeepAliveRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new KeepAliveRequestPacket { Id = buffer.ReadLong() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
