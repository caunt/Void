using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Plugins.ModsSupport.Forge.Packets;

namespace Void.Proxy.Plugins.ModsSupport.Forge.Services;

public class HandshakeService(IPlayerContext context) : IEventListener
{
    private readonly ConcurrentDictionary<IPlayer, HandshakePacket> _handshakes = [];

    [Subscribe]
    public void OnPlayerDisconnect(PlayerDisconnectedEvent @event)
    {
        _ = _handshakes.TryRemove(@event.Player, out _);
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshakePacket when handshakePacket.IsForge:
                var markers = handshakePacket.Markers;

                if (markers.Length > 1)
                    context.Logger.LogDebug("Has extra handshake markers {Markers}", string.Join(", ", markers));

                _handshakes[@event.Player] = handshakePacket;
                break;

            case PluginMessagePacket pluginMessagePacket:
                context.Logger.LogDebug("{Direction} Plugin Message {Channel} => {Data}", @event.Direction, pluginMessagePacket.Channel, Convert.ToHexString(pluginMessagePacket.Data.Span));
                break;

            case LoginPluginRequestPacket loginPluginRequestPacket:
                context.Logger.LogDebug("{Direction} Login Plugin Request {MessageId} => {Data}", @event.Direction, loginPluginRequestPacket.MessageId, Convert.ToHexString(loginPluginRequestPacket.Data));
                break;

            case LoginPluginResponsePacket loginPluginResponsePacket:
                context.Logger.LogDebug("{Direction} Login Plugin Response {MessageId} => {Data}", @event.Direction, loginPluginResponsePacket.MessageId, Convert.ToHexString(loginPluginResponsePacket.Data));
                break;

            case IMinecraftBinaryMessage minecraftBinaryMessage:
                break;

            default:
                if (@event.Player.Phase is not Phase.Play)
                    context.Logger.LogDebug("{Direction} {MessageType}", @event.Direction, @event.Message.GetType().Name);
                break;
        }
    }

    [Subscribe(PostOrder.First)]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        switch (@event)
        {
            case { Phase: Phase.Handshake, Side: Side.Client }:
                HandshakePacket.Register(context.Player);
                break;
            case { Phase: Phase.Login, Side: Side.Client }:
                LoginPluginResponsePacket.Register(context.Player);
                break;
            case { Phase: Phase.Login, Side: Side.Server }:
                LoginPluginRequestPacket.Register(context.Player);
                break;
            case { Phase: Phase.Play, Side: Side.Client }:
                context.Player.RegisterPacket<PluginMessagePacket>(Direction.Serverbound, [
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
                break;
            case { Phase: Phase.Play, Side: Side.Server }:
                context.Player.RegisterPacket<PluginMessagePacket>(Direction.Clientbound, [
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
}
