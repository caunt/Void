using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class BundleDelimiterPacket : IMinecraftClientboundPacket<BundleDelimiterPacket>
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
    }

    public static BundleDelimiterPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new BundleDelimiterPacket();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}