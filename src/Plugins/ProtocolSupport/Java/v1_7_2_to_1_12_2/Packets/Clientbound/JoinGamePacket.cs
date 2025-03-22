using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class JoinGamePacket : IClientboundPacket<JoinGamePacket>
{
    public required int EntityId { get; set; }
    public required short Gamemode { get; set; }
    public required int Dimension { get; set; }
    public required short Difficulty { get; set; }
    public required bool IsHardcore { get; set; }
    public required int MaxPlayers { get; set; }
    public required string? LevelType { get; set; }
    public required bool ReducedDebugInfo { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        EncodeLegacy(ref buffer, protocolVersion);
    }

    public static JoinGamePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return DecodeLegacy(ref buffer, protocolVersion);
    }

    private static JoinGamePacket DecodeLegacy(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var entityId = buffer.ReadInt();
        var gamemode = (short)buffer.ReadUnsignedByte();
        var isHardcore = (gamemode & 0x08) != 0;
        gamemode &= ~0x08;

        var dimension = protocolVersion < ProtocolVersion.MINECRAFT_1_9_1 ?
            buffer.ReadUnsignedByte() :
            buffer.ReadInt();

        var difficulty = buffer.ReadUnsignedByte();
        var maxPlayers = buffer.ReadUnsignedByte();
        var levelType = buffer.ReadString(16);

        var reducedDebugInfo = buffer.ReadBoolean();

        return new JoinGamePacket
        {
            EntityId = entityId,
            Gamemode = gamemode,
            IsHardcore = isHardcore,
            Dimension = dimension,
            Difficulty = difficulty,
            MaxPlayers = maxPlayers,
            LevelType = levelType,
            ReducedDebugInfo = reducedDebugInfo
        };
    }

    private void EncodeLegacy(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteInt(EntityId);
        buffer.WriteUnsignedByte((byte)(IsHardcore ? Gamemode | 0x8 : Gamemode));

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_9_1)
            buffer.WriteInt(Dimension);
        else
            buffer.WriteUnsignedByte((byte)Dimension);

        buffer.WriteUnsignedByte((byte)Difficulty);
        buffer.WriteUnsignedByte((byte)MaxPlayers);

        if (LevelType == null)
            throw new Exception("No level type specified.");

        buffer.WriteString(LevelType);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteBoolean(ReducedDebugInfo);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
