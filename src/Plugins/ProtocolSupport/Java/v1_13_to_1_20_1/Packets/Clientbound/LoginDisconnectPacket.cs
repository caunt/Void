using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class LoginDisconnectPacket : IMinecraftClientboundPacket<LoginDisconnectPacket>
{
    public required string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Reason);
    }

    public static LoginDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginDisconnectPacket { Reason = buffer.ReadString() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}