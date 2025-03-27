using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class LoginDisconnectPacket : IMinecraftClientboundPacket<LoginDisconnectPacket>
{
    public required string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Reason);
    }

    public static LoginDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginDisconnectPacket
        {
            Reason = buffer.ReadString()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}