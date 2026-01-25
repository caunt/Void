using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.Common.Network.Packets.Serverbound;

public class HandshakePacket : IMinecraftServerboundPacket<HandshakePacket>
{
    public required int ProtocolVersion { get; set; }
    public required string ServerAddress { get; set; }
    public required ushort ServerPort { get; set; }
    public required int NextState { get; set; }
    public bool IsStatusQuery => NextState == 1;

    public static HandshakePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new HandshakePacket
        {
            ProtocolVersion = buffer.ReadVarInt(),
            ServerAddress = buffer.ReadString(maxLength: 255),
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
        GC.SuppressFinalize(this);
    }
}
