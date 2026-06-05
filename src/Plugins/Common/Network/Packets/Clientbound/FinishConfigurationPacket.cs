using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

public class FinishConfigurationPacket : IMinecraftClientboundPacket<FinishConfigurationPacket>
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        // No data to encode
    }

    public static FinishConfigurationPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new FinishConfigurationPacket();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
