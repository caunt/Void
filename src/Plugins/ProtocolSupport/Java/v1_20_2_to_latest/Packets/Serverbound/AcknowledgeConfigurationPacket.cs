using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class AcknowledgeConfigurationPacket : IServerboundPacket<AcknowledgeConfigurationPacket>
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
    }

    public static AcknowledgeConfigurationPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new AcknowledgeConfigurationPacket();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}