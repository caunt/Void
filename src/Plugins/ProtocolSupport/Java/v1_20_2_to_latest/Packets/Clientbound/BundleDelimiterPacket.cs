using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class BundleDelimiterPacket : IClientboundPacket<BundleDelimiterPacket>
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