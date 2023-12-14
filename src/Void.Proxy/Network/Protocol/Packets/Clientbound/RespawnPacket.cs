using Void.Proxy.Models.Minecraft.Game;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;
using Void.Proxy.Utils.WaxNBT;
using System.Text;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

public struct RespawnPacket : IMinecraftPacket<PlayState>
{
    public int Dimension { get; set; }
    public long PartialHashedSeed { get; set; }
    public short Difficulty { get; set; }
    public short Gamemode { get; set; }
    public string? LevelType { get; set; }
    public byte DataToKeep { get; set; } // 1.16+
    public DimensionInfo DimensionInfo { get; set; } // 1.16-1.16.1
    public short PreviousGamemode { get; set; } // 1.16+
    public byte[]? CurrentDimensionData { get; set; } // 1.16.2+
    public KeyValuePair<string, long>? LastDeathPosition { get; set; } // 1.19+
    public int PortalCooldown { get; set; } // 1.20+

    public static RespawnPacket FromJoinGame(JoinGamePacket packet) => new RespawnPacket
    {
        Dimension = packet.Dimension,
        PartialHashedSeed = packet.PartialHashedSeed,
        Difficulty = packet.Difficulty,
        Gamemode = packet.Gamemode,
        LevelType = packet.LevelType,
        DataToKeep = 0,
        DimensionInfo = packet.DimensionInfo,
        PreviousGamemode = packet.PreviousGamemode,
        CurrentDimensionData = packet.CurrentDimensionData,
        LastDeathPosition = packet.LastDeathPosition,
        PortalCooldown = packet.PortalCooldown
    };

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
        {
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2 && protocolVersion < ProtocolVersion.MINECRAFT_1_19)
            {
                buffer.Write(CurrentDimensionData);
                buffer.WriteString(DimensionInfo.RegistryIdentifier);
            }
            else
            {
                buffer.WriteString(DimensionInfo.RegistryIdentifier);
                buffer.WriteString(DimensionInfo.LevelName);
            }
        }
        else
        {
            buffer.WriteInt(Dimension);
        }

        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_13_2)
            buffer.WriteUnsignedByte((byte)Difficulty);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            buffer.WriteLong(PartialHashedSeed);

        buffer.WriteUnsignedByte((byte)Gamemode);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
        {
            buffer.WriteUnsignedByte((byte)PreviousGamemode);
            buffer.WriteBoolean(DimensionInfo.IsDebugType);
            buffer.WriteBoolean(DimensionInfo.IsFlat);

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_3)
                buffer.WriteUnsignedByte(DataToKeep);
            else
                buffer.WriteBoolean(DataToKeep != 0);
        }
        else
        {
            buffer.WriteString(LevelType ?? string.Empty);
        }

        // optional death location
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (LastDeathPosition.HasValue)
            {
                var lastDeathPosition = LastDeathPosition.Value;

                buffer.WriteBoolean(true);
                buffer.WriteString(lastDeathPosition.Key);
                buffer.WriteLong(lastDeathPosition.Value);
            }
            else
            {
                buffer.WriteBoolean(false);
            }
        }

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20)
            buffer.WriteVarInt(PortalCooldown);
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var dimensionIdentifier = string.Empty;
        var levelName = string.Empty;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
        {
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2 && protocolVersion < ProtocolVersion.MINECRAFT_1_19)
            {
                CurrentDimensionData = ((MemoryStream)NbtFile.Parse(buffer.Span[buffer.Position..].ToArray()).Serialize()).ToArray();
                dimensionIdentifier = buffer.ReadString();
            }
            else
            {
                dimensionIdentifier = buffer.ReadString();
                levelName = buffer.ReadString();
            }
        }
        else
        {
            Dimension = buffer.ReadInt();
        }

        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_13_2)
            Difficulty = buffer.ReadUnsignedByte();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            PartialHashedSeed = buffer.ReadLong();

        Gamemode = buffer.ReadUnsignedByte();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
        {
            PreviousGamemode = buffer.ReadUnsignedByte();

            var isDebug = buffer.ReadBoolean();
            var isFlat = buffer.ReadBoolean();

            DimensionInfo = new DimensionInfo(dimensionIdentifier, levelName, isFlat, isDebug);

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_3)
                DataToKeep = buffer.ReadUnsignedByte();
            else if (buffer.ReadBoolean())
                DataToKeep = 1;
            else
                DataToKeep = 0;
        }
        else
        {
            LevelType = buffer.ReadString(16);
        }

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && buffer.ReadBoolean())
            LastDeathPosition = new(buffer.ReadString(), buffer.ReadLong());

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20)
            PortalCooldown = buffer.ReadVarInt();
    }

    public int MaxSize() => 0
        + 5 // Dimension
        + 8 // PartialHashedSeed
        + 2 // Difficulty
        + 2 // Gamemode
        + (LevelType is null ? 1 : Encoding.UTF8.GetByteCount(LevelType) + 5)
        + 1 // DataToKeep 
        + Encoding.UTF8.GetByteCount(DimensionInfo.LevelName) + 5
        + Encoding.UTF8.GetByteCount(DimensionInfo.RegistryIdentifier) + 5
        + 1 // DimensionInfo.IsDebugType
        + 1 // DimensionInfo.IsFlat
        + 2 // PreviousGamemode 
        + (CurrentDimensionData?.Length ?? 0)
        + (LastDeathPosition.HasValue ? Encoding.UTF8.GetByteCount(LastDeathPosition.Value.Key) : 0) + 5
        + (LastDeathPosition.HasValue ? 8 : 0)
        + 4; // PortalCooldown
}