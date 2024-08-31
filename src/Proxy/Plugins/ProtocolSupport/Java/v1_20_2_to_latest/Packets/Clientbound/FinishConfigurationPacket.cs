using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class FinishConfigurationPacket : IMinecraftPacket<FinishConfigurationPacket>
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
    }

    public static FinishConfigurationPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new FinishConfigurationPacket();
    }

    public void Dispose()
    {
    }
}