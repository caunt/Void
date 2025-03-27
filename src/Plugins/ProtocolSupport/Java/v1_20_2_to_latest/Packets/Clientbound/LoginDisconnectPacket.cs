using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class LoginDisconnectPacket : IMinecraftClientboundPacket<LoginDisconnectPacket>
{
    public required Component Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Reason, ProtocolVersion.MINECRAFT_1_20_2);
    }

    public static LoginDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginDisconnectPacket { Reason = buffer.ReadComponent(ProtocolVersion.MINECRAFT_1_20_2) };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}