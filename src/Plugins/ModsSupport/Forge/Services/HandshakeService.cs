using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Events;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Extensions;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.ModsSupport.Forge.Packets;

namespace Void.Proxy.Plugins.ModsSupport.Forge.Services;

public class HandshakeService(IPlayerContext context, IPluginService plugins, Plugin plugin) : IEventListener
{
    private class PlayerCachedMods
    {
        public required ModdedHandshakePacket HandshakePacket { get; set; }
        public TaskCompletionSource<ModdedLoginPluginResponsePacket>? PendingLoginPluginResponse { get; set; }
        public int LoginPluginRequestCounter { get; set; }
        public Dictionary<int, byte[]> FmlHandshakeResponses { get; set; } = [];
    }

    private readonly ConcurrentDictionary<IPlayer, PlayerCachedMods> _playerMods = [];

    [Subscribe]
    public void OnPlayerDisconnect(PlayerDisconnectedEvent @event)
    {
        _ = _playerMods.TryRemove(@event.Player, out _);
    }

    [Subscribe]
    public void OnServerboundHandshakeBuild(HandshakeBuildEvent @event)
    {
        if (!_playerMods.TryGetValue(@event.Player, out var mods))
            return;

        @event.Result = new HandshakeBuildEventResult(mods.HandshakePacket, mods.HandshakePacket.NextState);
    }

    [Subscribe]
    public async ValueTask OnLoginPluginMessageEvent(LoginPluginMessageEvent @event, CancellationToken cancellationToken)
    {
        const string LoginWrapperChannel = "fml:loginwrapper";
        const string HandshakeChannel = "fml:handshake";

        if (!_playerMods.TryGetValue(@event.Player, out var mods))
            return;

        if (@event is { Channel: LoginWrapperChannel })
        {
            var buffer = new BufferSpan(@event.Data);

            var channel = buffer.ReadString();
            var length = buffer.ReadVarInt();
            var id = buffer.ReadVarInt();
            var data = buffer.Read(length - id.VarIntSize());

            if (buffer.Remaining > 0)
                throw new InvalidOperationException($"Extra data remaining after parsing {LoginWrapperChannel} message.");

            context.Logger.LogTrace("Received Login Wrapper request message: Channel={Channel}, Id={Id}, Data={Data}", channel, id, Convert.ToHexString(data));

            // Forge ignores first Plugin Message Request and does not respond to it. Why tho? (tested on 1.20)
            var ignored = channel is HandshakeChannel && id is 5;

            if (@event.Player.Phase is Phase.Play)
            {
                if (ignored)
                    return;

                if (!mods.FmlHandshakeResponses.TryGetValue(id, out var response))
                    throw new InvalidOperationException($"No cached FML handshake response found for player {@event.Player} with request id {id}.");

                @event.Response = response;
                @event.Successful = true;
                @event.Result = true;
                return;
            }

            if (!ignored)
            {
                if (mods is { PendingLoginPluginResponse.Task.IsCompleted: false })
                    throw new InvalidOperationException($"A new login plugin message request in {@event.Channel} to player {@event.Player} received, while it is already pending to response.");

                mods.PendingLoginPluginResponse = new TaskCompletionSource<ModdedLoginPluginResponsePacket>();
            }

            await @event.Link.SendPacketAsync(new ModdedLoginPluginRequestPacket { MessageId = GetLoginPluginRequestIdFor(@event.Player), Channel = @event.Channel, Data = @event.Data }, cancellationToken);

            if (!ignored && mods.PendingLoginPluginResponse is { } pendingLoginPluginResponse)
            {
                // Receive system-defined login plugin message response so it triggers MessageReceivedEvent for modded login plugin message response
                _ = await @event.Link.ReceivePacketAsync<IMinecraftServerboundPacket>(cancellationToken);

                var moddedLoginPluginResponsePacket = await pendingLoginPluginResponse.Task;

                @event.Response = moddedLoginPluginResponsePacket.Data;
                @event.Successful = moddedLoginPluginResponsePacket.Successful;
                @event.Result = true;

                mods.FmlHandshakeResponses[id] = moddedLoginPluginResponsePacket.Data;
                context.Logger.LogTrace("Received Login Wrapper response message: Channel={Channel}, Id={Id}, Successful={Successful}, Response={Data}", channel, id, moddedLoginPluginResponsePacket.Successful, Convert.ToHexString(moddedLoginPluginResponsePacket.Data));
            }
        }
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        switch (@event.Message)
        {
            case ModdedHandshakePacket handshakePacket when handshakePacket.IsForge:
                var markers = handshakePacket.Markers;

                if (markers.Length > 1)
                    context.Logger.LogDebug("Has extra handshake markers {Markers}", string.Join(", ", markers));

                _playerMods[@event.Player] = new PlayerCachedMods { HandshakePacket = handshakePacket };
                break;

            case PluginMessagePacket pluginMessagePacket:
                context.Logger.LogTrace("{Direction} Plugin Message {Channel} => {Data}", @event.Direction, pluginMessagePacket.Channel, Convert.ToHexString(pluginMessagePacket.Data.Span));
                break;

            case ModdedLoginPluginRequestPacket loginPluginRequestPacket:
                context.Logger.LogTrace("{Direction} Login Plugin Request {MessageId} => {Data}", @event.Direction, loginPluginRequestPacket.MessageId, Convert.ToHexString(loginPluginRequestPacket.Data));
                break;

            case ModdedLoginPluginResponsePacket loginPluginResponsePacket:
                context.Logger.LogTrace("{Direction} Login Plugin Response {MessageId} => {Data}", @event.Direction, loginPluginResponsePacket.MessageId, Convert.ToHexString(loginPluginResponsePacket.Data));

                if (_playerMods.TryGetValue(@event.Player, out var mods) && mods is { PendingLoginPluginResponse.Task.IsCompleted: false })
                    mods.PendingLoginPluginResponse.SetResult(loginPluginResponsePacket);
                break;

            case IMinecraftBinaryMessage minecraftBinaryMessage:
                break;

            default:
                if (@event.Player.Phase is not Phase.Play && plugins.TryGetPluginFromType(@event.Message.GetType(), out var messagePlugin) && messagePlugin == plugin)
                    context.Logger.LogDebug("{Direction} {MessageType}", @event.Direction, @event.Message.GetType().FullName);
                break;
        }
    }

    [Subscribe(PostOrder.First)]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        switch (@event)
        {
            case { Phase: Phase.Handshake }:
                @event.Player.RegisterPacket<ModdedHandshakePacket>(
                    @event.Channel,
                    @event.Side switch
                    {
                        Side.Client => Operation.Read,
                        Side.Server => Operation.Write,
                        _ => throw new InvalidOperationException($"Invalid side changed phase: {@event.Side}")
                    },
                    [new(0x00, ProtocolVersion.Oldest)]);
                break;

            case { Phase: Phase.Login }:
                @event.Player.RegisterPacket<ModdedLoginPluginResponsePacket>(
                    @event.Channel,
                    @event.Side switch
                    {
                        Side.Client => Operation.Read,
                        Side.Server => Operation.Write,
                        _ => throw new InvalidOperationException($"Invalid side changed phase: {@event.Side}")
                    },
                    [new(0x02, ProtocolVersion.Oldest)]);
                @event.Player.RegisterPacket<ModdedLoginPluginRequestPacket>(
                    @event.Channel,
                    @event.Side switch
                    {
                        Side.Client => Operation.Write,
                        Side.Server => Operation.Read,
                        _ => throw new InvalidOperationException($"Invalid side changed phase: {@event.Side}")
                    },
                    [new(0x04, ProtocolVersion.Oldest)]);
                break;
            case { Phase: Phase.Play }:
                @event.Player.RegisterPacket<PluginMessagePacket>(
                    @event.Channel,
                    @event.Side switch
                    {
                        Side.Client => Operation.Read,
                        Side.Server => Operation.Write,
                        _ => throw new InvalidOperationException($"Invalid side changed phase: {@event.Side}")
                    },
                    [
                        new(0x17, ProtocolVersion.MINECRAFT_1_7_2),
                        new(0x09, ProtocolVersion.MINECRAFT_1_9),
                        new(0x0A, ProtocolVersion.MINECRAFT_1_12),
                        new(0x09, ProtocolVersion.MINECRAFT_1_12_1),
                        new(0x0A, ProtocolVersion.MINECRAFT_1_13),
                        new(0x0B, ProtocolVersion.MINECRAFT_1_14),
                        new(0x0A, ProtocolVersion.MINECRAFT_1_17),
                        new(0x0C, ProtocolVersion.MINECRAFT_1_19),
                        new(0x0D, ProtocolVersion.MINECRAFT_1_19_1),
                        new(0x0C, ProtocolVersion.MINECRAFT_1_19_3),
                        new(0x0D, ProtocolVersion.MINECRAFT_1_19_4),
                        new(0x0F, ProtocolVersion.MINECRAFT_1_20_2),
                        new(0x10, ProtocolVersion.MINECRAFT_1_20_3),
                        new(0x12, ProtocolVersion.MINECRAFT_1_20_5),
                        new(0x14, ProtocolVersion.MINECRAFT_1_21_2),
                        new(0x15, ProtocolVersion.MINECRAFT_1_21_6)
                    ]);
                @event.Player.RegisterPacket<PluginMessagePacket>(
                    @event.Channel,
                    @event.Side switch
                    {
                        Side.Client => Operation.Write,
                        Side.Server => Operation.Read,
                        _ => throw new InvalidOperationException($"Invalid side changed phase: {@event.Side}")
                    },
                    [
                        new(0x3F, ProtocolVersion.MINECRAFT_1_7_2),
                        new(0x18, ProtocolVersion.MINECRAFT_1_9),
                        new(0x19, ProtocolVersion.MINECRAFT_1_13),
                        new(0x18, ProtocolVersion.MINECRAFT_1_14),
                        new(0x19, ProtocolVersion.MINECRAFT_1_15),
                        new(0x18, ProtocolVersion.MINECRAFT_1_16),
                        new(0x17, ProtocolVersion.MINECRAFT_1_16_2),
                        new(0x18, ProtocolVersion.MINECRAFT_1_17),
                        new(0x15, ProtocolVersion.MINECRAFT_1_19),
                        new(0x16, ProtocolVersion.MINECRAFT_1_19_1),
                        new(0x15, ProtocolVersion.MINECRAFT_1_19_3),
                        new(0x17, ProtocolVersion.MINECRAFT_1_19_4),
                        new(0x18, ProtocolVersion.MINECRAFT_1_20_2),
                        new(0x19, ProtocolVersion.MINECRAFT_1_20_5),
                        new(0x18, ProtocolVersion.MINECRAFT_1_21_5)
                    ]);
                break;
        }
    }

    private int GetLoginPluginRequestIdFor(IPlayer player)
    {
        return _playerMods[player].LoginPluginRequestCounter++;
    }
}
