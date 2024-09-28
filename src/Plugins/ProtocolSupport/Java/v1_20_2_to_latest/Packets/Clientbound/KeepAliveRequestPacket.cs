using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class KeepAliveRequestPacket : IMinecraftPacket<KeepAliveRequestPacket>
{
    public long Id { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteLong(Id);
    }

    public static KeepAliveRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new KeepAliveRequestPacket
        {
            Id = buffer.ReadLong()
        };
    }

    public void Dispose()
    {
    }
}