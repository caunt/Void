using Void.NBT;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Mojang.Minecraft.World;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class JoinGamePacket : IClientboundPacket<JoinGamePacket>
{
    public required int EntityId { get; set; }
    public required short Gamemode { get; set; }
    public required int Dimension { get; set; }
    public required long PartialHashedSeed { get; set; } // 1.15+
    public required short Difficulty { get; set; }
    public required bool IsHardcore { get; set; }
    public required int MaxPlayers { get; set; }
    public required string? LevelType { get; set; }
    public required int ViewDistance { get; set; } // 1.14+
    public required bool ReducedDebugInfo { get; set; }
    public required bool ShowRespawnScreen { get; set; }
    public required bool DoLimitedCrafting { get; set; } // 1.20.2+
    public required string[] LevelNames { get; set; } // 1.16+
    public required byte[] Registry { get; set; } // 1.16+
    public required DimensionInfo? DimensionInfo { get; set; } // 1.16+
    public required byte[] CurrentDimensionData { get; set; } // 1.16.2+
    public required short PreviousGamemode { get; set; } // 1.16+
    public required int SimulationDistance { get; set; } // 1.18+
    public required KeyValuePair<string, long> LastDeathPosition { get; set; } // 1.19+
    public required int PortalCooldown { get; set; } // 1.20+

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            Encode116Up(ref buffer, protocolVersion);
        else
            EncodeLegacy(ref buffer, protocolVersion);
    }

    public static JoinGamePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            return Decode116Up(ref buffer, protocolVersion);
        else
            return DecodeLegacy(ref buffer, protocolVersion);
    }

    private static JoinGamePacket DecodeLegacy(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var entityId = buffer.ReadInt();
        var gamemode = (short)buffer.ReadUnsignedByte();
        var isHardcore = (gamemode & 0x08) != 0;
        gamemode &= ~0x08;

        var dimension = buffer.ReadInt();

        var difficulty = default(short);
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_13_2)
            difficulty = buffer.ReadUnsignedByte();

        var partialHashedSeed = default(long);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            partialHashedSeed = buffer.ReadLong();

        var maxPlayers = buffer.ReadUnsignedByte();
        var levelType = buffer.ReadString(16);

        var viewDistance = default(int);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_14)
            viewDistance = buffer.ReadVarInt();

        var reducedDebugInfo = buffer.ReadBoolean();

        var showRespawnScreen = default(bool);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_15)
            showRespawnScreen = buffer.ReadBoolean();

        return new JoinGamePacket
        {
            EntityId = entityId,
            Gamemode = gamemode,
            IsHardcore = isHardcore,
            Dimension = dimension,
            Difficulty = difficulty,
            PartialHashedSeed = partialHashedSeed,
            MaxPlayers = maxPlayers,
            LevelType = levelType,
            ViewDistance = viewDistance,
            ReducedDebugInfo = reducedDebugInfo,
            ShowRespawnScreen = showRespawnScreen,

            // fields not present in legacy
            DoLimitedCrafting = false,
            LevelNames = [],
            Registry = [],
            DimensionInfo = null,
            CurrentDimensionData = [],
            PreviousGamemode = 0,
            SimulationDistance = 0,
            LastDeathPosition = default,
            PortalCooldown = 0
        };
    }

    private static JoinGamePacket Decode116Up(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var entityId = buffer.ReadInt();

        var isHardcore = default(bool);
        var gamemode = default(short);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2)
        {
            isHardcore = buffer.ReadBoolean();
            gamemode = buffer.ReadUnsignedByte();
        }
        else
        {
            gamemode = buffer.ReadUnsignedByte();
            isHardcore = (gamemode & 0x08) != 0;
            gamemode &= ~0x08;
        }

        var previousGamemode = buffer.ReadUnsignedByte();

        var levelNames = new string[buffer.ReadVarInt()];
        for (var i = 0; i < levelNames.Length; i++)
            levelNames[i] = buffer.ReadString();

        var bufferPosition = buffer.Position;
        var reader = new NbtReader(buffer.ReadToEnd().ToArray()); // TODO remove the allocation
        var registry = ((MemoryStream)NbtFile.Parse(reader).Serialize()).ToArray();
        buffer.Seek(bufferPosition + reader.Position);

        var dimensionIdentifier = string.Empty;
        var levelName = string.Empty;
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2 && protocolVersion < ProtocolVersion.MINECRAFT_1_19)
        {
            bufferPosition = buffer.Position;
            reader = new NbtReader(buffer.ReadToEnd().ToArray());// TODO remove the allocation
            var currentDimensionData = ((MemoryStream)NbtFile.Parse(reader).Serialize()).ToArray();
            buffer.Seek(bufferPosition + reader.Position);

            dimensionIdentifier = buffer.ReadString();
        }
        else
        {
            dimensionIdentifier = buffer.ReadString();
            levelName = buffer.ReadString();
        }

        var partialHashedSeed = buffer.ReadLong();

        var maxPlayers = default(int);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16_2)
            maxPlayers = buffer.ReadVarInt();
        else
            maxPlayers = buffer.ReadUnsignedByte();

        var viewDistance = buffer.ReadVarInt();

        var simulationDistance = default(int);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_18)
            simulationDistance = buffer.ReadVarInt();

        var reducedDebugInfo = buffer.ReadBoolean();
        var showRespawnScreen = buffer.ReadBoolean();

        var isDebug = buffer.ReadBoolean();
        var isFlat = buffer.ReadBoolean();
        var dimensionInfo = new DimensionInfo(dimensionIdentifier, levelName, isFlat, isDebug);

        // optional death location
        var lastDeathPosition = default(KeyValuePair<string, long>);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19 && buffer.ReadBoolean())
            lastDeathPosition = new KeyValuePair<string, long>(buffer.ReadString(), buffer.ReadLong());

        var portalCooldown = default(int);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20)
            portalCooldown = buffer.ReadVarInt();

        return new JoinGamePacket
        {
            EntityId = entityId,
            IsHardcore = isHardcore,
            Gamemode = gamemode,
            PreviousGamemode = previousGamemode,
            LevelNames = levelNames,
            Registry = registry,
            PartialHashedSeed = partialHashedSeed,
            MaxPlayers = maxPlayers,
            ViewDistance = viewDistance,
            SimulationDistance = simulationDistance,
            ReducedDebugInfo = reducedDebugInfo,
            ShowRespawnScreen = showRespawnScreen,
            DimensionInfo = dimensionInfo,
            LastDeathPosition = lastDeathPosition,
            PortalCooldown = portalCooldown,

            // fields not present in 1.16+
            Dimension = 0,
            LevelType = null,
            Difficulty = 0,
            DoLimitedCrafting = false,
            CurrentDimensionData = []
        };
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
        if (DimensionInfo is null)
            throw new Exception("DimensionInfo was not set");

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

        buffer.WriteVarInt(LevelNames.Length);
        foreach (var levelName in LevelNames)
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
            buffer.WriteString(DimensionInfo.LevelName ?? string.Empty);
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

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (LastDeathPosition.IsDefault())
            {
                buffer.WriteBoolean(false);
            }
            else
            {
                buffer.WriteBoolean(true);
                buffer.WriteString(LastDeathPosition.Key);
                buffer.WriteLong(LastDeathPosition.Value);
            }
        }

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20)
            buffer.WriteVarInt(PortalCooldown);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
