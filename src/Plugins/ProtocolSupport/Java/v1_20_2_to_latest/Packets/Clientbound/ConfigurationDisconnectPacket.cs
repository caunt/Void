using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class ConfigurationDisconnectPacket : IMinecraftClientboundPacket<ConfigurationDisconnectPacket>
{
    public required Component Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Reason);
    }

    public static ConfigurationDisconnectPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new ConfigurationDisconnectPacket { Reason = buffer.ReadComponent() };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
