using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Plugins.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class HandshakePacket : IMinecraftPacket<HandshakePacket>
{
    public required int ProtocolVersion { get; set; }
    public required string ServerAddress { get; set; }
    public required ushort ServerPort { get; set; }
    public required int NextState { get; set; }

    public static HandshakePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new HandshakePacket
        {
            ProtocolVersion = buffer.ReadVarInt(),
            ServerAddress = buffer.ReadString(255 /* + forgeMarker*/),
            ServerPort = buffer.ReadUnsignedShort(),
            NextState = buffer.ReadVarInt()
        };
    }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(ProtocolVersion);
        buffer.WriteString(ServerAddress);
        buffer.WriteUnsignedShort(ServerPort);
        buffer.WriteVarInt(NextState);
    }

    public void Dispose()
    {
    }
}