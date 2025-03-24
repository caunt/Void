using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class StartConfigurationPacket : IMinecraftClientboundPacket<StartConfigurationPacket>
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