using MinecraftProxy.Models;
using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public class PlayerInfoUpdatePacket : IMinecraftPacket<PlayState>
{
    public List<PlayerInfo> Players { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        // if no players available, notchian server still send actions bitset, we dont?
        var actions = Players.SelectMany(player => player.Actions.Select(action => action.Type));
        var actionsFlags = (byte)actions.Aggregate(0, (current, action) => current | (byte)action);

        buffer.WriteUnsignedByte(actionsFlags);
        buffer.WriteVarInt(Players.Count);

        foreach (var player in Players)
        {
            var missingActions = actions.Where(actionType => player.Actions.All(action => action.Type != actionType));

            if (missingActions.Any())
                throw new Exception($"Player {player.Guid} has missing actions: {string.Join(", ", missingActions)}");

            buffer.WriteGuid(player.Guid);

            foreach (var action in player.Actions)
                action.Encode(buffer);
        }
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        var actionsFlags = buffer.ReadUnsignedByte();
        var actions = Enum.GetValues(typeof(PlayerActionType)).Cast<PlayerActionType>().Where(action => (actionsFlags & (byte)action) != 0).ToList();
        
        Players = Enumerable.Range(0, buffer.ReadVarInt()).Select(_ =>
        {
            return new PlayerInfo(buffer.ReadGuid(), actions.Select<PlayerActionType, IPlayerAction>(action => action switch
            {
                PlayerActionType.AddPlayer => new AddPlayerAction(buffer),
                PlayerActionType.InitializeChat => new InitializeChatAction(buffer),
                PlayerActionType.UpdateGameMode => new UpdateGameModeAction(buffer),
                PlayerActionType.UpdateListed => new UpdateListedAction(buffer),
                PlayerActionType.UpdateLatency => new UpdateLatencyAction(buffer),
                PlayerActionType.UpdateDisplayName => new UpdateDisplayNameAction(buffer),
                _ => throw new ArgumentOutOfRangeException($"Unknown update player action type {action}")
            }).ToList());
        }).ToList();
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

    public record PlayerInfo(Guid Guid, List<IPlayerAction> Actions);

    public interface IPlayerAction
    {
        public PlayerActionType Type { get; }
        public void Decode(MinecraftBuffer buffer);
        public void Encode(MinecraftBuffer buffer);
    }

    public abstract class AbstractPlayerAction : IPlayerAction
    {
        public PlayerActionType Type { get; }

        public AbstractPlayerAction(PlayerActionType type, MinecraftBuffer buffer)
        {
            Type = type;
            Decode(buffer);
        }

        public abstract void Decode(MinecraftBuffer buffer);
        public abstract void Encode(MinecraftBuffer buffer);
    }

    public class AddPlayerAction(MinecraftBuffer buffer) : AbstractPlayerAction(PlayerActionType.AddPlayer, buffer)
    {
        public string Name { get; set; }
        public List<Property> Properties { get; set; }

        public override void Decode(MinecraftBuffer buffer)
        {
            Name = buffer.ReadString();
            Properties = buffer.ReadPropertyList();
        }

        public override void Encode(MinecraftBuffer buffer)
        {
            buffer.WriteString(Name);
            buffer.WritePropertyList(Properties);
        }
    }

    public class InitializeChatAction(MinecraftBuffer buffer) : AbstractPlayerAction(PlayerActionType.InitializeChat, buffer)
    {
        public bool HasSignature { get; set; }
        public Guid SessionId { get; set; }
        public IdentifiedKey IdentifiedKey { get; set; }

        public override void Decode(MinecraftBuffer buffer)
        {
            HasSignature = buffer.ReadBoolean();

            if (!HasSignature)
                return;

            SessionId = buffer.ReadGuid();
            IdentifiedKey = buffer.ReadIdentifiedKey();
        }

        public override void Encode(MinecraftBuffer buffer)
        {
            buffer.WriteBoolean(HasSignature);

            if (!HasSignature)
                return;

            buffer.WriteGuid(SessionId);
            buffer.WriteIdentifiedKey(IdentifiedKey);
        }
    }

    public class UpdateGameModeAction(MinecraftBuffer buffer) : AbstractPlayerAction(PlayerActionType.UpdateGameMode, buffer)
    {
        public int GameMode { get; set; }

        public override void Decode(MinecraftBuffer buffer)
        {
            GameMode = buffer.ReadVarInt();
        }

        public override void Encode(MinecraftBuffer buffer)
        {
            buffer.WriteVarInt(GameMode);
        }
    }

    public class UpdateListedAction(MinecraftBuffer buffer) : AbstractPlayerAction(PlayerActionType.UpdateListed, buffer)
    {
        public bool Listed { get; set; }
    
        public override void Decode(MinecraftBuffer buffer)
        {
            Listed = buffer.ReadBoolean();
        }

        public override void Encode(MinecraftBuffer buffer)
        {
            buffer.WriteBoolean(Listed);
        }
    }

    public class UpdateLatencyAction(MinecraftBuffer buffer) : AbstractPlayerAction(PlayerActionType.UpdateLatency, buffer)
    {
        public int Ping { get; set; }
    
        public override void Decode(MinecraftBuffer buffer)
        {
            Ping = buffer.ReadVarInt();
        }

        public override void Encode(MinecraftBuffer buffer)
        {
            buffer.WriteVarInt(Ping);
        }
    }

    public class UpdateDisplayNameAction(MinecraftBuffer buffer) : AbstractPlayerAction(PlayerActionType.UpdateDisplayName, buffer)
    {
        public bool HasDisplayName { get; set; }
        public string DisplayName { get; set; }

        public override void Decode(MinecraftBuffer buffer)
        {
            HasDisplayName = buffer.ReadBoolean();

            if (!HasDisplayName)
                return;

            DisplayName = buffer.ReadString();
        }

        public override void Encode(MinecraftBuffer buffer)
        {
            buffer.WriteBoolean(HasDisplayName);

            if (!HasDisplayName)
                return;

            buffer.WriteString(DisplayName);
        }
    }
}