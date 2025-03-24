using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class AcknowledgeConfigurationPacket : IMinecraftServerboundPacket<AcknowledgeConfigurationPacket>
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