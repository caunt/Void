using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class KeepAliveResponsePacket : IServerboundPacket<KeepAliveResponsePacket>
{
    public long Id { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteLong(Id);
    }

    public static KeepAliveResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new KeepAliveResponsePacket
        {
            Id = buffer.ReadLong()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}