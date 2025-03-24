using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class LoginAcknowledgedPacket : IMinecraftServerboundPacket<LoginAcknowledgedPacket>
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
    }

    public static LoginAcknowledgedPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new LoginAcknowledgedPacket();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}