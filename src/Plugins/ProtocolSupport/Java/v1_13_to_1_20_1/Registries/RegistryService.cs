using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Channels;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Links;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels.Extensions;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Services.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

public class RegistryService(ILogger<RegistryService> logger, IPlugin plugin, IPlayerService players, ILinkService links, IEventService events) : AbstractRegistryService(logger, plugin, players, links, events)
{
    private readonly IEventService _events = events;
    private readonly IPlugin _plugin = plugin;

    [Subscribe]
    public async ValueTask OnChannelCreated(ChannelCreatedEvent @event, CancellationToken cancellationToken)
    {
        if (!Plugin.SupportedVersions.Contains(@event.Player.ProtocolVersion))
            return;

        var systemRegistry = @event.Channel.GetSystemPacketRegistryHolder();
        var pluginsRegistry = @event.Channel.GetPluginsPacketRegistryHolder();

        if (!systemRegistry.IsEmpty || !pluginsRegistry.IsEmpty)
            return;

        systemRegistry.ManagedBy = _plugin;
        systemRegistry.ProtocolVersion = @event.Player.ProtocolVersion;

        pluginsRegistry.ManagedBy = _plugin;
        pluginsRegistry.ProtocolVersion = @event.Player.ProtocolVersion;

        if (@event.Side is Side.Client)
        {
            @event.Channel.SetReadingPacketsMappings(_plugin, Registry.ServerboundHandshakeMappings);
            @event.Channel.SetWritingPacketsMappings(_plugin, Registry.ClientboundHandshakeMappings);
        }
        else
        {
            @event.Channel.SetReadingPacketsMappings(_plugin, Registry.ClientboundHandshakeMappings);
            @event.Channel.SetWritingPacketsMappings(_plugin, Registry.ServerboundHandshakeMappings);
        }

        await @event.Player.SetPhaseAsync(@event.Side, Phase.Handshake, @event.Channel, cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                Debug.Assert(Plugin.SupportedVersions.Contains(ProtocolVersion.Get(handshake.ProtocolVersion)));

                if (handshake.NextState is 1)
                {
                    @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundStatusMappings);
                    @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundStatusMappings);
                    await @event.Link.Player.SetPhaseAsync(Side.Client, Phase.Status, @event.Link.PlayerChannel, cancellationToken);
                }
                else if (handshake.NextState is 2 or 3)
                {
                    @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundLoginMappings);
                    @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundLoginMappings);
                    await @event.Link.Player.SetPhaseAsync(Side.Client, Phase.Login, @event.Link.PlayerChannel, cancellationToken);
                }

                break;
            case LoginSuccessPacket:
                @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundPlayMappings);
                @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundPlayMappings);
                await @event.Link.Player.SetPhaseAsync(Side.Server, Phase.Play, @event.Link.ServerChannel, cancellationToken);
                break;
        }
    }

    [Subscribe]
    public async ValueTask OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                if (handshake.NextState is 1)
                {
                    @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundStatusMappings);
                    @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundStatusMappings);
                    await @event.Link.Player.SetPhaseAsync(Side.Server, Phase.Status, @event.Link.ServerChannel, cancellationToken);
                }
                else if (handshake.NextState is 2 or 3)
                {
                    @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundLoginMappings);
                    @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundLoginMappings);
                    await @event.Link.Player.SetPhaseAsync(Side.Server, Phase.Login, @event.Link.ServerChannel, cancellationToken);
                }
                else
                {
                    @event.Link.PlayerChannel.ClearPacketsMappings(_plugin);
                    @event.Link.ServerChannel.ClearPacketsMappings(_plugin);
                }

                break;
            case LoginSuccessPacket:
                @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundPlayMappings);
                @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundPlayMappings);
                await @event.Link.Player.SetPhaseAsync(Side.Client, Phase.Play, @event.Link.PlayerChannel, cancellationToken);
                break;
        }
    }

    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }
}