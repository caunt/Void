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
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Mojang;

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
            var playState = await ReceivePlayerHandshakeAsync(@event.Link, cancellationToken) is 2 or 3;

            if (playState)
                @event.Result = AuthenticationSide.Proxy;
        }
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

        var authenticationResult = await AuthenticatePlayerAsync(@event.Link, cancellationToken);

        if (authenticationResult is AuthenticationResult.Authenticated or AuthenticationResult.AlreadyAuthenticated)
            authenticationResult = await AuthenticateServerAsync(@event.Link, authenticationResult, cancellationToken);

        @event.Result = authenticationResult;
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
            return AuthenticationResult.NotAuthenticatedPlayer;

        if (!await events.ThrowWithResultAsync(new PlayerVerifyingEncryptionEvent(link.Player, link), cancellationToken))
            return AuthenticationResult.NotAuthenticatedPlayer;

        if (!await VerifyMojangProfile(link.Player, cancellationToken))
            return AuthenticationResult.NotAuthenticatedPlayer;

        await events.ThrowAsync(new PlayerVerifiedEncryptionEvent(link.Player, link), cancellationToken);
        await FinishPlayerLoginAsync(link, cancellationToken);

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

        var handshakeBuildEventResult = await events.ThrowWithResultAsync(new HandshakeBuildEvent(link.Player, link), cancellationToken) ?? new(null, 2);

        await HandshakeWithServerAsync(link, handshakeBuildEventResult.Packet, handshakeBuildEventResult.NextState, cancellationToken);
        await events.ThrowAsync(new HandshakeCompletedEvent(link.Player, link, handshakeBuildEventResult.NextState), cancellationToken);
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

            if (result is AuthenticationResult.NoResult)
                continue;
            else if (result is AuthenticationResult.Authenticated)
                break;
            else
                return AuthenticationResult.NotAuthenticatedServer;
        }

        await FinishServerAuthenticationAsync(link, authenticationResult, cancellationToken);
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

    protected abstract ValueTask<int> ReceivePlayerHandshakeAsync(ILink link, CancellationToken cancellationToken);

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
