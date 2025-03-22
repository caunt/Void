using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class StartConfigurationPacket : IClientboundPacket<StartConfigurationPacket>
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
    }

    public static StartConfigurationPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new StartConfigurationPacket();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}