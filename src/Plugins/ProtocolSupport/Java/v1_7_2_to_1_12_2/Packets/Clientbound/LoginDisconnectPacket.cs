using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class LoginDisconnectPacket : IClientboundPacket<LoginDisconnectPacket>
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