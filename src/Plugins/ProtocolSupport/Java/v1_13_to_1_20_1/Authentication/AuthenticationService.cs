﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events.Authentication;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Bundles;
using Void.Proxy.Plugins.Common.Services.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Authentication;

public class AuthenticationService(ILogger<AuthenticationService> logger, IEventService events, IPlayerService players) : AbstractAuthenticationService(events, players)
{
    private readonly IEventService _events = events;

    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override async ValueTask<bool> IsPlayerAuthenticatedAsync(IPlayer player, CancellationToken cancellationToken)
    {
        return await player.IsAuthenticatedAsync(cancellationToken);
    }

    protected override async ValueTask<bool> IsPlayerPlayingAsync(IPlayer player, CancellationToken cancellationToken)
    {
        return await player.IsPlayingAsync(cancellationToken);
    }

    protected override async ValueTask<int> GetHandshakeNextStateAsync(ILink link, CancellationToken cancellationToken)
    {
        var handshake = await link.ReceivePacketAsync<HandshakePacket>(cancellationToken);

        // Status query
        if (handshake.NextState is not 2 and not 3)
            await link.SendPacketAsync(handshake, cancellationToken);

        return handshake.NextState;
    }

    protected override async ValueTask FinishPlayingAsync(ILink link, CancellationToken cancellationToken)
    {
        // IPlayer might be in middle of Bundle when client is not handling packets until closing bundle received
        var bundles = link.Player.Context.Services.GetRequiredService<IBundleService>();

        if (bundles.IsActivated)
            await link.SendPacketAsync<BundleDelimiterPacket>(cancellationToken);
    }

    protected override async ValueTask<bool> IdentifyPlayerAsync(ILink link, CancellationToken cancellationToken)
    {
        if (!link.Player.TryGetMinecraftPlayer(out var player))
            return false;

        var loginStart = await link.ReceivePacketAsync<LoginStartPacket>(cancellationToken);

        if (IsAlreadyOnline(loginStart.Profile.Username))
            return false;

        player.Profile = loginStart.Profile;
        player.IdentifiedKey = loginStart.Key;
        return true;
    }

    protected override async ValueTask AdmitPlayerAsync(ILink link, CancellationToken cancellationToken)
    {
        if (!link.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (player.Profile is null)
            throw new InvalidOperationException("Player should be identified before admitting");

        await link.SendPacketAsync(new LoginSuccessPacket
        {
            GameProfile = player.Profile
        }, cancellationToken);
    }

    protected override async ValueTask PrepareServerAuthenticationAsync(ILink link, CancellationToken cancellationToken)
    {
        if (!link.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (player.Profile is null)
            throw new InvalidOperationException("Player should be admitted before preparing server");

        await link.SendPacketAsync(new HandshakePacket
        {
            NextState = 2,
            ProtocolVersion = player.ProtocolVersion.Version,
            ServerAddress = link.Server.Host,
            ServerPort = (ushort)link.Server.Port
        }, cancellationToken);

        await link.SendPacketAsync(new LoginStartPacket
        {
            Profile = player.Profile,
            Key = player.IdentifiedKey
        }, cancellationToken);
    }

    protected override async ValueTask FinishServerAuthenticationAsync(ILink link, AuthenticationResult authenticationResult, CancellationToken cancellationToken)
    {
        var joinGamePacket = await link.ReceivePacketAsync<JoinGamePacket>(cancellationToken);

        if (authenticationResult is AuthenticationResult.AlreadyAuthenticated)
            await link.SendPacketAsync(RespawnPacket.FromJoinGame(joinGamePacket), cancellationToken);
        else
            await link.SendPacketAsync(joinGamePacket, cancellationToken);
    }

    protected override async ValueTask<AuthenticationResult> HandleServerPacketAsync(ILink link, IMinecraftClientboundPacket packet, CancellationToken cancellationToken)
    {
        switch (packet)
        {
            case LoginPluginRequestPacket loginPluginRequestPacket:
                var result = await _events.ThrowWithResultAsync(new LoginPluginRequestEvent(link, loginPluginRequestPacket.Channel, loginPluginRequestPacket.Data), cancellationToken);
                await link.SendPacketAsync(new LoginPluginResponsePacket { Successful = result is not null, Data = result ?? [], MessageId = loginPluginRequestPacket.MessageId }, cancellationToken);
                break;
            case SetCompressionPacket:
                // handled by compression service
                break;
            case LoginDisconnectPacket loginDisconnectPacket:
                logger.LogInformation("Player {Player} cannot authenticate on {Server}: {Reason}", link.Player, link.Server, loginDisconnectPacket.Reason.SerializeLegacy());

                // since IPlayer client is already completed login state, it cannot be kicked with login disconnect packet
                await link.Player.KickAsync(loginDisconnectPacket.Reason, cancellationToken);
                return AuthenticationResult.NotAuthenticatedServer;
            case LoginSuccessPacket:
                return AuthenticationResult.Authenticated;
            default:
                throw new InvalidOperationException($"Unexpected {packet} packet received");
        }

        return AuthenticationResult.NoResult;
    }
}
