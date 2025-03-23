using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Authentication;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Links;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Mojang.Services;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Players;
using Void.Proxy.API.Players.Extensions;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Mojang;

namespace Void.Proxy.Plugins.Common.Services.Authentication;

public abstract class AbstractAuthenticationService(IEventService events, IPlayerService players) : IPluginService
{
    [Subscribe]
    public static void OnPlayerConnecting(PlayerConnectingEvent @event)
    {
        if (!@event.Services.HasService<IMojangService>())
            @event.Services.AddSingleton<IMojangService, MojangService>();
    }

    [Subscribe]
    public async ValueTask OnAuthenticationStarting(AuthenticationStartingEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        if (!await @event.Link.Player.IsProtocolSupportedAsync(cancellationToken))
        {
            @event.Result = AuthenticationSide.Server;
            return;
        }

        if (await IsPlayerAuthenticatedAsync(@event.Link.Player, cancellationToken))
        {
            // since IPlayer is already authenticated, we have no choice but to continue authenticating IServer on proxy side
            @event.Result = AuthenticationSide.Proxy;

            if (await IsPlayerPlayingAsync(@event.Link.Player, cancellationToken))
                await FinishPlayingAsync(@event.Link, cancellationToken);
        }
        else
        {
            var playState = await GetHandshakeNextStateAsync(@event.Link, cancellationToken) is 2 or 3;

            if (playState)
                @event.Result = AuthenticationSide.Proxy;
        }
    }

    [Subscribe]
    public async ValueTask OnAuthenticationStarted(AuthenticationStartedEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        if (!await @event.Link.Player.IsProtocolSupportedAsync(cancellationToken))
            return;

        if (@event.Side is AuthenticationSide.Server)
        {
            @event.Result = AuthenticationResult.Authenticated;
            return;
        }

        var authenticationResult = await AuthenticatePlayerAsync(@event.Link, cancellationToken);

        if (authenticationResult is AuthenticationResult.Authenticated or AuthenticationResult.AlreadyAuthenticated)
            authenticationResult = await AuthenticateServerAsync(@event.Link, authenticationResult, cancellationToken);

        @event.Result = authenticationResult;
    }

    protected bool IsAlreadyOnline(string username)
    {
        return players.All.Any(player => player.Profile?.Username == username);
    }

    protected async ValueTask<AuthenticationResult> AuthenticatePlayerAsync(ILink link, CancellationToken cancellationToken)
    {
        if (await IsPlayerAuthenticatedAsync(link.Player, cancellationToken))
            return AuthenticationResult.AlreadyAuthenticated;

        if (!await IdentifyPlayerAsync(link, cancellationToken))
            return AuthenticationResult.NotAuthenticatedPlayer;

        if (!await events.ThrowWithResultAsync(new PlayerVerifyingEncryptionEvent(link), cancellationToken))
            return AuthenticationResult.NotAuthenticatedPlayer;

        await events.ThrowAsync(new PlayerVerifiedEncryptionEvent(link), cancellationToken);
        await AdmitPlayerAsync(link, cancellationToken);

        return AuthenticationResult.Authenticated;
    }

    protected async ValueTask<AuthenticationResult> AuthenticateServerAsync(ILink link, AuthenticationResult authenticationResult, CancellationToken cancellationToken)
    {
        // server channel might be closed very early, skip authentication, ILink should stop itself as soon as its executes
        if (!link.ServerChannel.IsAlive)
            return AuthenticationResult.Authenticated;

        if (link.Player.Profile is null)
            throw new InvalidOperationException("Player should be authenticated before Server");

        await PrepareServerAuthenticationAsync(link, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var packet = await link.ReceivePacketAsync<IMinecraftClientboundPacket>(cancellationToken);
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

    protected abstract ValueTask<int> GetHandshakeNextStateAsync(ILink link, CancellationToken cancellationToken);

    protected static async ValueTask VerifyMojangProfile(IPlayer player, ReadOnlyMemory<byte> sharedSecret, CancellationToken cancellationToken)
    {
        var mojang = player.Context.Services.GetRequiredService<IMojangService>();

        if (await mojang.VerifyAsync(player, sharedSecret, cancellationToken) is not { } onlineProfile)
            throw new NotSupportedException("Playing in offline-mode is not supported yet. Cannot verify profile.");

        player.Profile = onlineProfile;
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion version);

    protected abstract ValueTask<bool> IsPlayerAuthenticatedAsync(IPlayer player, CancellationToken cancellationToken);

    protected abstract ValueTask<bool> IsPlayerPlayingAsync(IPlayer player, CancellationToken cancellationToken);

    protected abstract ValueTask<bool> IdentifyPlayerAsync(ILink link, CancellationToken cancellationToken);

    protected abstract ValueTask AdmitPlayerAsync(ILink link, CancellationToken cancellationToken);

    protected abstract ValueTask PrepareServerAuthenticationAsync(ILink link, CancellationToken cancellationToken);

    protected abstract ValueTask<AuthenticationResult> HandleServerPacketAsync(ILink link, IMinecraftClientboundPacket packet, CancellationToken cancellationToken);
}