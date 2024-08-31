using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class LoginAcknowledgedPacket : IMinecraftPacket<LoginAcknowledgedPacket>
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
    }
}