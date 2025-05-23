﻿using Microsoft.Extensions.Logging;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Services.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

public class RegistryService(ILogger<RegistryService> logger, Plugin plugin, IPlayerService players, ILinkService links, IEventService events) : AbstractRegistryService(logger, plugin, players, links, events)
{
    private readonly IPlugin _plugin = plugin;

    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (@event.Message is HandshakePacket packet && !Plugin.SupportedVersions.Contains(ProtocolVersion.Get(packet.ProtocolVersion)))
            return;

        switch (@event.Message)
        {
            case HandshakePacket handshake:
                if (handshake.NextState is 1)
                {
                    @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundStatusMappings);
                    @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundStatusMappings);
                    await @event.Player.SetPhaseAsync(Side.Client, Phase.Status, @event.Link.PlayerChannel, cancellationToken);
                }
                else if (handshake.NextState is 2 or 3)
                {
                    @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundLoginMappings);
                    @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundLoginMappings);
                    await @event.Player.SetPhaseAsync(Side.Client, Phase.Login, @event.Link.PlayerChannel, cancellationToken);
                }

                break;
            case LoginSuccessPacket:
                @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundPlayMappings);
                @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundPlayMappings);
                await @event.Player.SetPhaseAsync(Side.Server, Phase.Play, @event.Link.ServerChannel, cancellationToken);
                break;
        }
    }

    [Subscribe]
    public async ValueTask OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        switch (@event.Message)
        {
            case HandshakePacket handshake:
                if (handshake.NextState is 1)
                {
                    @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundStatusMappings);
                    @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundStatusMappings);
                    await @event.Player.SetPhaseAsync(Side.Server, Phase.Status, @event.Link.ServerChannel, cancellationToken);
                }
                else if (handshake.NextState is 2 or 3)
                {
                    @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundLoginMappings);
                    @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundLoginMappings);
                    await @event.Player.SetPhaseAsync(Side.Server, Phase.Login, @event.Link.ServerChannel, cancellationToken);
                }
                else
                {
                    @event.Link.PlayerChannel.DisposeRegistries(_plugin);
                    @event.Link.ServerChannel.DisposeRegistries(_plugin);
                }

                break;
            case LoginSuccessPacket:
                @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundPlayMappings);
                @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundPlayMappings);
                await @event.Player.SetPhaseAsync(Side.Client, Phase.Play, @event.Link.PlayerChannel, cancellationToken);
                break;
        }
    }

    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override void SetupRegistries(INetworkChannel channel, Side side, ProtocolVersion protocolVersion)
    {
        channel.GetRegistries().Setup(_plugin, protocolVersion);

        if (side is Side.Client)
        {
            channel.SetReadingPacketsMappings(_plugin, Registry.ServerboundHandshakeMappings);
            channel.SetWritingPacketsMappings(_plugin, Registry.ClientboundHandshakeMappings);
        }
        else
        {
            channel.SetReadingPacketsMappings(_plugin, Registry.ClientboundHandshakeMappings);
            channel.SetWritingPacketsMappings(_plugin, Registry.ServerboundHandshakeMappings);
        }
    }
}
