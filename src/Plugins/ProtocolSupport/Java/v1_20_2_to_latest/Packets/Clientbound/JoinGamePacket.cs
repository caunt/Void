using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.World;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class JoinGamePacket : IMinecraftClientboundPacket<JoinGamePacket>
{
    public required int EntityId { get; set; }
    public required bool IsHardcore { get; set; }
    public required string[] LevelNames { get; set; }
    public required int MaxPlayers { get; set; }
    public required int ViewDistance { get; set; }
    public required int SimulationDistance { get; set; }
    public required bool ReducedDebugInfo { get; set; }
    public required bool ShowRespawnScreen { get; set; }
    public required bool DoLimitedCrafting { get; set; }
    public required int Dimension { get; set; }
    public required DimensionInfo? DimensionInfo { get; set; }
    public required long PartialHashedSeed { get; set; }
    public required short Gamemode { get; set; }
    public required short PreviousGamemode { get; set; }
    public required KeyValuePair<string, long> LastDeathPosition { get; set; }
    public required int PortalCooldown { get; set; }
    public required int SeaLevel { get; set; }
    public required bool OnlineMode { get; set; }
    public required bool EnforcesSecureChat { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (DimensionInfo is null)
            throw new InvalidOperationException($"{nameof(DimensionInfo)} was not set.");

        buffer.WriteInt(EntityId);
        buffer.WriteBoolean(IsHardcore);

        buffer.WriteVarInt(LevelNames.Length);
        foreach (var levelName in LevelNames)
            buffer.WriteString(levelName);

        buffer.WriteVarInt(MaxPlayers);
        buffer.WriteVarInt(ViewDistance);
        buffer.WriteVarInt(SimulationDistance);

        buffer.WriteBoolean(ReducedDebugInfo);
        buffer.WriteBoolean(ShowRespawnScreen);
        buffer.WriteBoolean(DoLimitedCrafting);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
            buffer.WriteVarInt(Dimension);
        else
            buffer.WriteString(DimensionInfo.RegistryIdentifier);

        buffer.WriteString(DimensionInfo.LevelName ?? string.Empty);
        buffer.WriteLong(PartialHashedSeed);

        buffer.WriteUnsignedByte(unchecked((byte)Gamemode));
        buffer.WriteUnsignedByte(unchecked((byte)PreviousGamemode));

        buffer.WriteBoolean(DimensionInfo.IsDebugType);
        buffer.WriteBoolean(DimensionInfo.IsFlat);

        if (LastDeathPosition.Key is null)
        {
            buffer.WriteBoolean(false);
        }
        else
        {
            buffer.WriteBoolean(true);
            buffer.WriteString(LastDeathPosition.Key);
            buffer.WriteLong(LastDeathPosition.Value);
        }

        buffer.WriteVarInt(PortalCooldown);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_21_2)
            buffer.WriteVarInt(SeaLevel);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_26_2)
            buffer.WriteBoolean(OnlineMode);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
            buffer.WriteBoolean(EnforcesSecureChat);
    }

    public static JoinGamePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var entityId = buffer.ReadInt();
        var isHardcore = buffer.ReadBoolean();

        var levelNames = new string[buffer.ReadVarInt()];
        for (var i = 0; i < levelNames.Length; i++)
            levelNames[i] = buffer.ReadString();

        var maxPlayers = buffer.ReadVarInt();
        var viewDistance = buffer.ReadVarInt();
        var simulationDistance = buffer.ReadVarInt();

        var reducedDebugInfo = buffer.ReadBoolean();
        var showRespawnScreen = buffer.ReadBoolean();
        var doLimitedCrafting = buffer.ReadBoolean();

        var dimension = default(int);
        var dimensionIdentifier = string.Empty;
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
            dimension = buffer.ReadVarInt();
        else
            dimensionIdentifier = buffer.ReadString();

        var levelName = buffer.ReadString();
        var partialHashedSeed = buffer.ReadLong();

        var gamemode = (short)(sbyte)buffer.ReadUnsignedByte();
        var previousGamemode = (short)(sbyte)buffer.ReadUnsignedByte();

        var isDebug = buffer.ReadBoolean();
        var isFlat = buffer.ReadBoolean();
        var dimensionInfo = new DimensionInfo(dimensionIdentifier, levelName, isFlat, isDebug);

        var lastDeathPosition = default(KeyValuePair<string, long>);
        if (buffer.ReadBoolean())
            lastDeathPosition = new KeyValuePair<string, long>(buffer.ReadString(), buffer.ReadLong());

        var portalCooldown = buffer.ReadVarInt();

        var seaLevel = default(int);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_21_2)
            seaLevel = buffer.ReadVarInt();

        var onlineMode = default(bool);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_26_2)
            onlineMode = buffer.ReadBoolean();

        var enforcesSecureChat = default(bool);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5)
            enforcesSecureChat = buffer.ReadBoolean();

        return new JoinGamePacket
        {
            EntityId = entityId,
            IsHardcore = isHardcore,
            LevelNames = levelNames,
            MaxPlayers = maxPlayers,
            ViewDistance = viewDistance,
            SimulationDistance = simulationDistance,
            ReducedDebugInfo = reducedDebugInfo,
            ShowRespawnScreen = showRespawnScreen,
            DoLimitedCrafting = doLimitedCrafting,
            Dimension = dimension,
            DimensionInfo = dimensionInfo,
            PartialHashedSeed = partialHashedSeed,
            Gamemode = gamemode,
            PreviousGamemode = previousGamemode,
            LastDeathPosition = lastDeathPosition,
            PortalCooldown = portalCooldown,
            SeaLevel = seaLevel,
            OnlineMode = onlineMode,
            EnforcesSecureChat = enforcesSecureChat
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
