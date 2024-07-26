using Void.Proxy.Models.Minecraft.Encryption;
using Void.Proxy.Models.Minecraft.Profile;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

public struct PlayerInfoUpdatePacket : IMinecraftPacket<PlayState>
{
    public List<PlayerInfo> Players { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        // if no players available, notchian server still send actions bitset, we dont?
        var actions = Players.SelectMany(player => player.Actions.Select(action => action.Type))
            .ToArray();
        var actionsFlags = (byte)actions.Aggregate(0, (current, action) => current | (byte)action);

        buffer.WriteUnsignedByte(actionsFlags);
        buffer.WriteVarInt(Players.Count);

        foreach (var player in Players)
        {
            var missingActions = actions.Where(actionType => player.Actions.All(action => action.Type != actionType))
                .ToArray();

            if (missingActions.Any())
                throw new Exception($"Player {player.Guid} has missing actions: {string.Join(", ", missingActions)}");

            buffer.WriteGuid(player.Guid);

            foreach (var action in player.Actions)
                action.Encode(ref buffer);
        }
    }

    public async Task<bool> HandleAsync(PlayState state)
    {
        return await state.HandleAsync(this);
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var actionsFlags = buffer.ReadUnsignedByte();
        var actions = Enum.GetValues(typeof(PlayerActionType))
            .Cast<PlayerActionType>()
            .Where(action => (actionsFlags & (byte)action) != 0)
            .ToList();

        var count = buffer.ReadVarInt();
        Players = new List<PlayerInfo>(count);

        for (var i = 0; i < count; i++)
        {
            var playerGuid = buffer.ReadGuid();
            var playerActions = new List<IPlayerAction>();

            foreach (var action in actions)
            {
                playerActions.Add(action switch
                {
                    PlayerActionType.AddPlayer => new AddPlayerAction(ref buffer, protocolVersion),
                    PlayerActionType.InitializeChat => new InitializeChatAction(ref buffer, protocolVersion),
                    PlayerActionType.UpdateGameMode => new UpdateGameModeAction(ref buffer, protocolVersion),
                    PlayerActionType.UpdateListed => new UpdateListedAction(ref buffer, protocolVersion),
                    PlayerActionType.UpdateLatency => new UpdateLatencyAction(ref buffer, protocolVersion),
                    PlayerActionType.UpdateDisplayName => new UpdateDisplayNameAction(ref buffer, protocolVersion),
                    _ => throw new ArgumentOutOfRangeException($"Unknown update player action type {action}")
                });
            }

            Players.Add(new PlayerInfo(playerGuid, playerActions));
        }
    }

    [Flags]
    public enum PlayerActionType
    {
        AddPlayer = 0x01,
        InitializeChat = 0x02,
        UpdateGameMode = 0x04,
        UpdateListed = 0x08,
        UpdateLatency = 0x10,
        UpdateDisplayName = 0x20
    }

    public record PlayerInfo(
        Guid Guid,
        List<IPlayerAction> Actions);

    public interface IPlayerAction
    {
        public PlayerActionType Type { get; }
        public void Decode(ref MinecraftBuffer buffer);
        public void Encode(ref MinecraftBuffer buffer);
    }

    public abstract class AbstractPlayerAction : IPlayerAction
    {
        public AbstractPlayerAction(PlayerActionType type, ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
        {
            Type = type;
            Decode(ref buffer);
        }

        public PlayerActionType Type { get; }

        public abstract void Decode(ref MinecraftBuffer buffer);
        public abstract void Encode(ref MinecraftBuffer buffer);
    }

    public class AddPlayerAction(
        ref MinecraftBuffer buffer,
        ProtocolVersion protocolVersion) : AbstractPlayerAction(PlayerActionType.AddPlayer, ref buffer, protocolVersion)
    {
        public string Name { get; set; }
        public List<Property> Properties { get; set; }

        public override void Decode(ref MinecraftBuffer buffer)
        {
            Name = buffer.ReadString();
            Properties = buffer.ReadPropertyList();
        }

        public override void Encode(ref MinecraftBuffer buffer)
        {
            buffer.WriteString(Name);
            buffer.WritePropertyList(Properties);
        }
    }

    public class InitializeChatAction(
        ref MinecraftBuffer buffer,
        ProtocolVersion protocolVersion) : AbstractPlayerAction(PlayerActionType.InitializeChat, ref buffer, protocolVersion)
    {
        public bool HasSignature { get; set; }
        public Guid SessionId { get; set; }
        public IdentifiedKey IdentifiedKey { get; set; }

        public override void Decode(ref MinecraftBuffer buffer)
        {
            HasSignature = buffer.ReadBoolean();

            if (!HasSignature)
                return;

            SessionId = buffer.ReadGuid();
            IdentifiedKey = buffer.ReadIdentifiedKey(protocolVersion);
        }

        public override void Encode(ref MinecraftBuffer buffer)
        {
            buffer.WriteBoolean(HasSignature);

            if (!HasSignature)
                return;

            buffer.WriteGuid(SessionId);
            buffer.WriteIdentifiedKey(IdentifiedKey);
        }
    }

    public class UpdateGameModeAction(
        ref MinecraftBuffer buffer,
        ProtocolVersion protocolVersion) : AbstractPlayerAction(PlayerActionType.UpdateGameMode, ref buffer, protocolVersion)
    {
        public int GameMode { get; set; }

        public override void Decode(ref MinecraftBuffer buffer)
        {
            GameMode = buffer.ReadVarInt();
        }

        public override void Encode(ref MinecraftBuffer buffer)
        {
            buffer.WriteVarInt(GameMode);
        }
    }

    public class UpdateListedAction(
        ref MinecraftBuffer buffer,
        ProtocolVersion protocolVersion) : AbstractPlayerAction(PlayerActionType.UpdateListed, ref buffer, protocolVersion)
    {
        public bool Listed { get; set; }

        public override void Decode(ref MinecraftBuffer buffer)
        {
            Listed = buffer.ReadBoolean();
        }

        public override void Encode(ref MinecraftBuffer buffer)
        {
            buffer.WriteBoolean(Listed);
        }
    }

    public class UpdateLatencyAction(
        ref MinecraftBuffer buffer,
        ProtocolVersion protocolVersion) : AbstractPlayerAction(PlayerActionType.UpdateLatency, ref buffer, protocolVersion)
    {
        public int Ping { get; set; }

        public override void Decode(ref MinecraftBuffer buffer)
        {
            Ping = buffer.ReadVarInt();
        }

        public override void Encode(ref MinecraftBuffer buffer)
        {
            buffer.WriteVarInt(Ping);
        }
    }

    public class UpdateDisplayNameAction(
        ref MinecraftBuffer buffer,
        ProtocolVersion protocolVersion) : AbstractPlayerAction(PlayerActionType.UpdateDisplayName, ref buffer, protocolVersion)
    {
        public bool HasDisplayName { get; set; }
        public string DisplayName { get; set; }

        public override void Decode(ref MinecraftBuffer buffer)
        {
            HasDisplayName = buffer.ReadBoolean();

            if (!HasDisplayName)
                return;

            DisplayName = buffer.ReadString();
        }

        public override void Encode(ref MinecraftBuffer buffer)
        {
            buffer.WriteBoolean(HasDisplayName);

            if (!HasDisplayName)
                return;

            buffer.WriteString(DisplayName);
        }
    }
}