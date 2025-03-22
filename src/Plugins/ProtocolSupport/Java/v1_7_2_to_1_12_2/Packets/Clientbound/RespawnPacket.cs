using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class RespawnPacket : IClientboundPacket<RespawnPacket>
{
    public required int Dimension { get; set; }
    public required short Difficulty { get; set; }
    public required short Gamemode { get; set; }
    public required string? LevelType { get; set; }

    public static RespawnPacket FromJoinGame(JoinGamePacket packet)
    {
        return new RespawnPacket
        {
            Dimension = packet.Dimension,
            Difficulty = packet.Difficulty,
            Gamemode = packet.Gamemode,
            LevelType = packet.LevelType
        };
    }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteInt(Dimension);
        buffer.WriteUnsignedByte((byte)Difficulty);
        buffer.WriteUnsignedByte((byte)Gamemode);
        buffer.WriteString(LevelType ?? string.Empty);
    }

    public static RespawnPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var dimension = buffer.ReadInt();
        var difficulty = buffer.ReadUnsignedByte();
        var gamemode = buffer.ReadUnsignedByte();
        var levelType = buffer.ReadString(16);

        return new RespawnPacket
        {
            Dimension = dimension,
            Difficulty = difficulty,
            Gamemode = gamemode,
            LevelType = levelType
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}