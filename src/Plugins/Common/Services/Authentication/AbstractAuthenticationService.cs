using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Mojang;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Authentication;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Mojang;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;

namespace Void.Proxy.Plugins.Common.Services.Authentication;

public abstract class AbstractAuthenticationService(IEventService events, IPlayerService players, IDependencyService dependencies) : IPluginCommonService
{
    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent _)
    {
        dependencies.Register(services => services.AddSingleton<IMojangService, MojangService>());
    }

    [Subscribe]
    public async ValueTask OnAuthenticationStarting(AuthenticationStartingEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (!await @event.Player.IsProtocolSupportedAsync(cancellationToken))
        {
            @event.Result = AuthenticationSide.Server;
            return;
        }

        if (await IsPlayerAuthenticatedAsync(@event.Player, cancellationToken))
        {
            // since IPlayer is already authenticated, we have no choice but to continue authenticating IServer on proxy side
            @event.Result = AuthenticationSide.Proxy;

            if (await IsPlayerPlayingAsync(@event.Player, cancellationToken))
                await FinishPlayingAsync(@event.Link, cancellationToken);
        }
        else
        {
            var handshake = await @event.Link.ReceivePacketAsync<HandshakePacket>(cancellationToken);

            // Status query
            if (handshake.IsStatusQuery)
                await @event.Link.SendPacketAsync(handshake, cancellationToken);

            await events.ThrowAsync(new HandshakeCompletedEvent(@event.Link.Player, @event.Link, Side.Client, handshake.ServerAddress, handshake.NextState), cancellationToken);

            if (!handshake.IsStatusQuery)
                @event.Result = AuthenticationSide.Proxy;
        }
    }


    [Subscribe]
    public async ValueTask OnPlayerKickEvent(PlayerKickEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event.Player.Phase is not Phase.Handshake)
            return;

        var channel = await @event.Player.GetChannelAsync(cancellationToken);

        if (channel.IsPausedRead)
            channel.Resume(Operation.Read);

        var handshake = await channel.ReceivePacketAsync<HandshakePacket>(cancellationToken);

        if (!handshake.IsStatusQuery)
            await @event.Player.SetPhaseAsync(link: null, Side.Client, Phase.Login, await @event.Player.GetChannelAsync(cancellationToken), cancellationToken);

        @event.Result = true;
    }

    [Subscribe]
    public async ValueTask OnAuthenticationStarted(AuthenticationStartedEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (@event.Side is AuthenticationSide.Server)
        {
            @event.Result = AuthenticationResult.Authenticated;
            return;
        }

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (!await @event.Player.IsProtocolSupportedAsync(cancellationToken))
            return;

        var playerAuthenticationResult = await AuthenticatePlayerAsync(@event.Link, cancellationToken);
        var serverAuthenticationResult = playerAuthenticationResult;

        if (playerAuthenticationResult.IsAuthenticated)
            serverAuthenticationResult = await AuthenticateServerAsync(@event.Link, playerAuthenticationResult, cancellationToken);

        if (serverAuthenticationResult.IsAuthenticated)
        {
            if (playerAuthenticationResult != AuthenticationResult.AlreadyAuthenticated)
                await FinishPlayerLoginAsync(@event.Link, cancellationToken);

            await FinishServerAuthenticationAsync(@event.Link, playerAuthenticationResult, cancellationToken);
        }

        @event.Result = serverAuthenticationResult;
    }

    protected bool IsAlreadyOnline(string username)
    {
        return players.All.Any(player => player.IsMinecraft && player.Profile?.Username == username);
    }

    protected async ValueTask<AuthenticationResult> AuthenticatePlayerAsync(ILink link, CancellationToken cancellationToken)
    {
        if (await IsPlayerAuthenticatedAsync(link.Player, cancellationToken))
            return AuthenticationResult.AlreadyAuthenticated;

        if (!await StartPlayerLoginAsync(link, cancellationToken))
            return AuthenticationResult.NotAuthenticatedPlayer with { Message = "You are already online on this proxy." };

        if (!await events.ThrowWithResultAsync(new PlayerVerifyingEncryptionEvent(link.Player, link), cancellationToken))
            return AuthenticationResult.NotAuthenticatedPlayer with { Message = "Your encryption cannot be verified." };

        if (!await VerifyMojangProfile(link.Player, cancellationToken))
            return AuthenticationResult.NotAuthenticatedPlayer with { Message = "Mojang session server rejected your session." };

        await events.ThrowAsync(new PlayerVerifiedEncryptionEvent(link.Player, link), cancellationToken);

        return AuthenticationResult.Authenticated;
    }

    protected async ValueTask<AuthenticationResult> AuthenticateServerAsync(ILink link, AuthenticationResult authenticationResult, CancellationToken cancellationToken)
    {
        if (!link.Player.IsMinecraft)
            return AuthenticationResult.Authenticated;

        // Server channel might be closed very early, skip authentication, ILink should stop itself as soon as it executes
        if (!link.ServerChannel.IsAlive)
            return AuthenticationResult.Authenticated;

        if (link.Player.Profile is null)
            throw new InvalidOperationException("Player should be authenticated before Server");

        var handshakeBuildEventResult = await events.ThrowWithResultAsync(new HandshakeBuildEvent(link.Player, link), cancellationToken) ?? new(NextState: 2);

        await HandshakeWithServerAsync(link, handshakeBuildEventResult.Packet, handshakeBuildEventResult.NextState, cancellationToken);
        await events.ThrowAsync(new HandshakeCompletedEvent(link.Player, link, Side.Server, handshakeBuildEventResult.ServerAddress ?? link.Server.Host, handshakeBuildEventResult.NextState), cancellationToken);
        await StartServerLoginAsync(link, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var packet = await link.ReceiveCancellablePacketAsync<IMinecraftClientboundPacket>(cancellationToken);

            if (packet is null)
                continue;

            if (packet is IMinecraftBinaryMessage message)
            {
                // TODO: What should we do with unknown messages that expect a response?
                // Client is already in the Play state, so it cannot answer these messages
                // Possible solution is to handle these packets in some "healing" service?
                // Happens when Velocity Forwarding plugin is not loaded
                await link.SendPacketAsync(message, cancellationToken);
                throw new NotSupportedException("This packet is not processed by anyone and might be waiting for a response");
                // continue;
            }

            var result = await HandleServerPacketAsync(link, packet, cancellationToken);

            if (result == AuthenticationResult.NoResult)
                continue;
            else if (result == AuthenticationResult.Authenticated)
                break;
            else
                return AuthenticationResult.NotAuthenticatedServer with { Message = "Disconnected by server." };
        }

        return AuthenticationResult.Authenticated;
    }

    protected virtual ValueTask FinishPlayingAsync(ILink link, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask FinishServerAuthenticationAsync(ILink link, AuthenticationResult authenticationResult, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    protected static async ValueTask<bool> VerifyMojangProfile(IPlayer player, CancellationToken cancellationToken)
    {
        if (!player.IsMinecraft)
            return false;

        var mojang = player.Context.Services.GetRequiredService<IMojangService>();

        if (await mojang.VerifyAsync(player, cancellationToken) is not { } onlineProfile)
            return false;

        player.Profile = onlineProfile;
        return true;
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion version);

    protected abstract ValueTask<bool> IsPlayerAuthenticatedAsync(IPlayer player, CancellationToken cancellationToken);

    protected abstract ValueTask<bool> IsPlayerPlayingAsync(IPlayer player, CancellationToken cancellationToken);

    protected abstract ValueTask<bool> StartPlayerLoginAsync(ILink link, CancellationToken cancellationToken);

    protected abstract ValueTask FinishPlayerLoginAsync(ILink link, CancellationToken cancellationToken);

    protected abstract ValueTask HandshakeWithServerAsync(ILink link, IMinecraftServerboundPacket? packet, int nextState, CancellationToken cancellationToken);

    protected abstract ValueTask StartServerLoginAsync(ILink link, CancellationToken cancellationToken);

    protected abstract ValueTask<AuthenticationResult> HandleServerPacketAsync(ILink link, IMinecraftClientboundPacket packet, CancellationToken cancellationToken);
}
