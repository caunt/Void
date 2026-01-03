using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class PlayDisconnectPacket : IMinecraftClientboundPacket<PlayDisconnectPacket>
{
    public required Component Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteJsonComponent(Reason);
    }

    public static PlayDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new PlayDisconnectPacket { Reason = buffer.ReadJsonComponent() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
