using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class PlayDisconnectPacket : IMinecraftClientboundPacket<PlayDisconnectPacket>
{
    public required Component Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Reason, protocolVersion);
    }

    public static PlayDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new PlayDisconnectPacket { Reason = buffer.ReadComponent(protocolVersion) };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}