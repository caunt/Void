using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class PlayDisconnectPacket : IMinecraftClientboundPacket<PlayDisconnectPacket>
{
    public required string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Reason);
    }

    public static PlayDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new PlayDisconnectPacket { Reason = buffer.ReadString() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}