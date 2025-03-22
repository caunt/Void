using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Channels;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Protocol;
using Void.Proxy.Plugins.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Services;

public class RegistryService(IPlugin plugin, IPlayerService players, IEventService events) : IPluginService
{
    [Subscribe]
    public void OnChannelCreated(ChannelCreatedEvent @event)
    {
        if (!Plugin.SupportedVersions.Contains(@event.Initiator.ProtocolVersion))
            return;

        var registry = @event.Channel.GetPacketRegistryHolder();

        if (!registry.IsEmpty)
            return;

        registry.ManagedBy = plugin;
        registry.ProtocolVersion = @event.Initiator.ProtocolVersion;

        SetRegistry(@event.Channel, Operation.Read, Mappings.ClientboundHandshakeMappings);
        SetRegistry(@event.Channel, Operation.Write, Mappings.ServerboundHandshakeMappings);
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                var playerProtocolVersion = ProtocolVersion.Get(handshake.ProtocolVersion);

                if (!Plugin.SupportedVersions.Contains(playerProtocolVersion))
                    break;

                SetRegistry(@event.Link.PlayerChannel, Operation.Read, Mappings.ClientboundLoginMappings);
                SetRegistry(@event.Link.ServerChannel, Operation.Write, Mappings.ClientboundLoginMappings);
                break;
            case LoginSuccessPacket:
                SetRegistry(@event.Link.PlayerChannel, Operation.Read, Mappings.ServerboundPlayMappings);
                SetRegistry(@event.Link.ServerChannel, Operation.Write, Mappings.ServerboundPlayMappings);
                break;
        }
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                if (handshake.NextState == 2)
                {
                    SetRegistry(@event.Link.PlayerChannel, Operation.Read, Mappings.ServerboundLoginMappings);
                    SetRegistry(@event.Link.ServerChannel, Operation.Write, Mappings.ServerboundLoginMappings);
                }
                else
                {
                    ClearRegistry(@event.Link.PlayerChannel);
                }

                break;
            case LoginSuccessPacket:
                SetRegistry(@event.Link.PlayerChannel, Operation.Read, Mappings.ClientboundPlayMappings);
                SetRegistry(@event.Link.ServerChannel, Operation.Write, Mappings.ClientboundPlayMappings);
                break;
        }
    }

    [Subscribe]
    public void OnPlayerDisconnected(PlayerDisconnectedEvent @event)
    {
        if (@event.Player.Context.Channel is { } channel)
            ClearRegistry(channel);
    }

    [Subscribe]
    public async ValueTask OnPluginUnload(PluginUnloadEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != plugin)
            return;

        events.UnregisterListeners(this);

        foreach (var player in players.All)
            ClearRegistry(await player.GetChannelAsync(cancellationToken));
    }

    private void SetRegistry(IMinecraftChannel channel, Operation operation, IReadOnlyDictionary<PacketMapping[], Type> mappings)
    {
        var registry = channel.GetPacketRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.ReplacePackets(operation, mappings);
    }

    private void ClearRegistry(IMinecraftChannel channel)
    {
        var registry = channel.GetPacketRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.Reset();
    }
}