using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.ModsSupport.Forge.Packets;

public class HandshakePacket : IMinecraftServerboundPacket<HandshakePacket>
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

    public static void Register(IPlayer player)
    {
        player.RegisterPacket<HandshakePacket>([new(0x00, Minecraft.Network.ProtocolVersion.Oldest)]);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
