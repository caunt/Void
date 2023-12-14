using Void.Proxy.Models.Minecraft.Game;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;
using Void.Proxy.Utils.WaxNBT;
using System.Text;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

public struct JoinGamePacket : IMinecraftPacket<PlayState>
{
    public int EntityId { get; set; }
    public short Gamemode { get; set; }
    public int Dimension { get; set; }
    public long PartialHashedSeed { get; set; } // 1.15+
    public short Difficulty { get; set; }
    public bool IsHardcore { get; set; }
    public int MaxPlayers { get; set; }
    public string? LevelType { get; set; }
    public int ViewDistance { get; set; } // 1.14+
    public bool ReducedDebugInfo { get; set; }
    public bool ShowRespawnScreen { get; set; }
    public bool DoLimitedCrafting { get; set; } // 1.20.2+
    public string[] levelNames { get; set; } // 1.16+
    public byte[]? Registry { get; set; } // 1.16+
    public DimensionInfo DimensionInfo { get; set; } // 1.16+
    public byte[]? CurrentDimensionData { get; set; } // 1.16.2+
    public short PreviousGamemode { get; set; } // 1.16+
    public int SimulationDistance { get; set; } // 1.18+
    public KeyValuePair<string, long>? LastDeathPosition { get; set; } // 1.19+
    public int PortalCooldown { get; set; } // 1.20+

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
            Encode1202Up(ref buffer, protocolVersion);
        else if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            Encode116Up(ref buffer, protocolVersion);
        else
            EncodeLegacy(ref buffer, protocolVersion);
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
            Decode1202Up(ref buffer, protocolVersion);
        else if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            Decode116Up(ref buffer, protocolVersion);
        else
            DecodeLegacy(ref buffer, protocolVersion);
    }

    public int MaxSize() => 0
        + 8 // EntityId
        + 2 // Gamemode
        + 4 // Dimension
        + 8 // PartialHashedSeed
        + 2 // Difficulty
        + 1 // IsHardcore
        + 4 // MaxPlayers
        + (LevelType is null ? 1 : Encoding.UTF8.GetByteCount(LevelType) + 5)
        + 4 // ViewDistance
        + 1 // ReducedDebugInfo
        + 1 // ShowRespawnScreen
        + 1 // DoLimitedCrafting
        + levelNames.Sum(levelName => Encoding.UTF8.GetByteCount(levelName) + 5)
        + (Registry?.Length ?? 0)
        + Encoding.UTF8.GetByteCount(DimensionInfo.LevelName) + 5
        + Encoding.UTF8.GetByteCount(DimensionInfo.RegistryIdentifier) + 5
        + 1 // DimensionInfo.IsDebugType
        + 1 // DimensionInfo.IsFlat
        + (CurrentDimensionData?.Length ?? 0)
        + 2 // PreviousGamemode
        + 4 // SimulationDistance
        + (LastDeathPosition.HasValue ? Encoding.UTF8.GetByteCount(LastDeathPosition.Value.Key) : 0) + 5
        + (LastDeathPosition.HasValue ? 8 : 0)
        + 4; // PortalCooldown*/

    private void DecodeLegacy(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        EntityId = buffer.ReadInt();
        Gamemode = buffer.ReadUnsignedByte();
        IsHardcore = (Gamemode & 0x08) != 0;
        Gamemode &= ~0x08;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_9_1)
            Dimension = buffer.ReadInt();
        else
            Dimension = buffer.ReadUnsignedByte();

        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_13_2)
            Difficulty = buffer.ReadUnsignedByte();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            PartialHashedSeed = buffer.ReadLong();

        MaxPlayers = buffer.ReadUnsignedByte();
        LevelType = buffer.ReadString(16);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_14)
            ViewDistance = buffer.ReadVarInt();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            ReducedDebugInfo = buffer.ReadBoolean();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            ShowRespawnScreen = buffer.ReadBoolean();
    }

    private void Decode116Up(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        EntityId = buffer.ReadInt();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2)
        {
            IsHardcore = buffer.ReadBoolean();
            Gamemode = buffer.ReadUnsignedByte();
        }
        else
        {
            Gamemode = buffer.ReadUnsignedByte();
            IsHardcore = (Gamemode & 0x08) != 0;
            Gamemode &= ~0x08;
        }

        PreviousGamemode = buffer.ReadUnsignedByte();

        levelNames = new string[buffer.ReadVarInt()];
        for (int i = 0; i < levelNames.Length; i++)
            levelNames[i] = buffer.ReadString();

        var reader = new NbtReader(buffer.Span[buffer.Position..].ToArray());
        Registry = ((MemoryStream)NbtFile.Parse(reader).Serialize()).ToArray();
        buffer.Seek(reader.Position);

        var dimensionIdentifier = string.Empty;
        var levelName = string.Empty;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2 && protocolVersion < ProtocolVersion.MINECRAFT_1_19)
        {
            reader = new NbtReader(buffer.Span[buffer.Position..].ToArray());
            CurrentDimensionData = ((MemoryStream)NbtFile.Parse(reader).Serialize()).ToArray();
            buffer.Seek(reader.Position);

            dimensionIdentifier = buffer.ReadString();
        }
        else
        {
            dimensionIdentifier = buffer.ReadString();
            levelName = buffer.ReadString();
        }

        PartialHashedSeed = buffer.ReadLong();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2)
            MaxPlayers = buffer.ReadVarInt();
        else
            MaxPlayers = buffer.ReadUnsignedByte();

        ViewDistance = buffer.ReadVarInt();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_18)
            SimulationDistance = buffer.ReadVarInt();

        ReducedDebugInfo = buffer.ReadBoolean();
        ShowRespawnScreen = buffer.ReadBoolean();

        var isDebug = buffer.ReadBoolean();
        var isFlat = buffer.ReadBoolean();
        DimensionInfo = new DimensionInfo(dimensionIdentifier, levelName, isFlat, isDebug);

        // optional death location
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && buffer.ReadBoolean())
            LastDeathPosition = new(buffer.ReadString(), buffer.ReadLong());

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20)
            PortalCooldown = buffer.ReadVarInt();
    }

    private void Decode1202Up(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        EntityId = buffer.ReadInt();
        IsHardcore = buffer.ReadBoolean();

        levelNames = new string[buffer.ReadVarInt()];
        for (int i = 0; i < levelNames.Length; i++)
            levelNames[i] = buffer.ReadString();

        MaxPlayers = buffer.ReadVarInt();

        ViewDistance = buffer.ReadVarInt();
        SimulationDistance = buffer.ReadVarInt();

        ReducedDebugInfo = buffer.ReadBoolean();
        ShowRespawnScreen = buffer.ReadBoolean();
        DoLimitedCrafting = buffer.ReadBoolean();

        var dimensionIdentifier = buffer.ReadString();
        var levelName = buffer.ReadString();
        PartialHashedSeed = buffer.ReadLong();

        Gamemode = buffer.ReadUnsignedByte();
        PreviousGamemode = buffer.ReadUnsignedByte();

        var isDebug = buffer.ReadBoolean();
        var isFlat = buffer.ReadBoolean();
        DimensionInfo = new(dimensionIdentifier, levelName, isFlat, isDebug);

        // optional death location
        if (buffer.ReadBoolean())
            LastDeathPosition = new(buffer.ReadString(), buffer.ReadLong());

        PortalCooldown = buffer.ReadVarInt();
    }

    private void EncodeLegacy(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteInt(EntityId);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2)
        {
            buffer.WriteBoolean(IsHardcore);
            buffer.WriteUnsignedByte((byte)Gamemode);
        }
        else
        {
            buffer.WriteUnsignedByte((byte)(IsHardcore ? Gamemode | 0x8 : Gamemode));
        }

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_9_1)
            buffer.WriteInt(Dimension);
        else
            buffer.WriteUnsignedByte((byte)Dimension);

        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_13_2)
            buffer.WriteUnsignedByte((byte)Difficulty);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            buffer.WriteLong(PartialHashedSeed);

        buffer.WriteUnsignedByte((byte)MaxPlayers);

        if (LevelType == null)
            throw new Exception("No level type specified.");

        buffer.WriteString(LevelType);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_14)
            buffer.WriteVarInt(ViewDistance);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteBoolean(ReducedDebugInfo);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            buffer.WriteBoolean(ShowRespawnScreen);
    }

    private void Encode116Up(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteInt(EntityId);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2)
        {
            buffer.WriteBoolean(IsHardcore);
            buffer.WriteUnsignedByte((byte)Gamemode);
        }
        else
        {
            buffer.WriteUnsignedByte((byte)(IsHardcore ? Gamemode | 0x8 : Gamemode));
        }

        buffer.WriteUnsignedByte((byte)PreviousGamemode);

        buffer.WriteVarInt(levelNames.Length);
        foreach (var levelName in levelNames)
            buffer.WriteString(levelName);

        buffer.Write(Registry);

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

        buffer.WriteLong(PartialHashedSeed);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2)
            buffer.WriteVarInt(MaxPlayers);
        else
            buffer.WriteUnsignedByte((byte)MaxPlayers);

        buffer.WriteVarInt(ViewDistance);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_18)
            buffer.WriteVarInt(SimulationDistance);

        buffer.WriteBoolean(ReducedDebugInfo);
        buffer.WriteBoolean(ShowRespawnScreen);

        buffer.WriteBoolean(DimensionInfo.IsDebugType);
        buffer.WriteBoolean(DimensionInfo.IsFlat);

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

    private void Encode1202Up(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteInt(EntityId);
        buffer.WriteBoolean(IsHardcore);

        buffer.WriteVarInt(levelNames.Length);
        foreach (var levelName in levelNames)
            buffer.WriteString(levelName);

        buffer.WriteVarInt(MaxPlayers);

        buffer.WriteVarInt(ViewDistance);
        buffer.WriteVarInt(SimulationDistance);

        buffer.WriteBoolean(ReducedDebugInfo);
        buffer.WriteBoolean(ShowRespawnScreen);
        buffer.WriteBoolean(DoLimitedCrafting);

        buffer.WriteString(DimensionInfo.RegistryIdentifier);
        buffer.WriteString(DimensionInfo.LevelName);
        buffer.WriteLong(PartialHashedSeed);

        buffer.WriteUnsignedByte((byte)Gamemode);
        buffer.WriteUnsignedByte((byte)PreviousGamemode);

        buffer.WriteBoolean(DimensionInfo.IsDebugType);
        buffer.WriteBoolean(DimensionInfo.IsFlat);

        // optional death location
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

        buffer.WriteVarInt(PortalCooldown);
    }
}
