using System.Diagnostics.CodeAnalysis;
using Nito.AsyncEx;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Authentication;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Servers;
using Void.Proxy.Players.Extensions;

namespace Void.Proxy.Links;

public class LinkService(ILogger<LinkService> logger, IServerService servers, IEventService events, IHostApplicationLifetime hostApplicationLifetime) : ILinkService, IEventListener
{
    private readonly List<ILink> _weakLinks = [];
    private readonly List<ILink> _activeLinks = [];
    private readonly AsyncLock _lock = new();

    public IReadOnlyList<ILink> All => _activeLinks.AsReadOnly();

    public async ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        return await ConnectPlayerAnywhereAsync(player, [], cancellationToken);
    }

    public async ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, IEnumerable<IServer> ignoredServers, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Looking for a server for {Player} player", player);

        var selectedServer = await events.ThrowWithResultAsync(new PlayerSearchServerEvent(player.Unwrap()), cancellationToken);

        if (selectedServer is not null)
        {
            if (ignoredServers.Contains(selectedServer))
                logger.LogWarning("Selected server {Server} for player {Player} is in ignored servers list", selectedServer, player);

            return await ConnectCoreAsync(player, selectedServer, cancellationToken);
        }

        foreach (var server in servers.All.Except(ignoredServers))
        {
            try
            {
                var result = await ConnectCoreAsync(player, server, cancellationToken);

                if (result is not ConnectionResult.Connected)
                    continue;

                return ConnectionResult.Connected;
            }
            catch (StreamException)
            {
                // Try next server
            }
        }

        return ConnectionResult.NotConnected;
    }

    public async ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        var previousServer = player.Server;
        var result = await ConnectCoreAsync(player, server, cancellationToken);

        if (result is ConnectionResult.NotConnected)
        {
            if (previousServer is null)
            {
                await player.KickAsync("Could not redirect you to the target server and you had no previous server.", cancellationToken);
                return result;
            }

            result = await ConnectCoreAsync(player, previousServer, cancellationToken);

            if (result is ConnectionResult.NotConnected)
            {
                await player.KickAsync("Could not redirect you to the target server nor previous server.", cancellationToken);
                return result;
            }
        }

        return result;
    }

    [Subscribe(PostOrder.First)]
    public async ValueTask OnLinkStopped(LinkStoppedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Link.IsAlive)
            throw new Exception($"Link {@event.Link} is still alive");

        using (await _lock.LockAsync(cancellationToken))
        {
            if (!_activeLinks.Remove(@event.Link))
                throw new InvalidOperationException($"Link {@event.Link} is not found");

            _weakLinks.Remove(@event.Link);
        }

        // IServer channel is no longer needed
        @event.Link.ServerChannel.Close();

        if (!@event.Link.PlayerChannel.IsAlive)
            @event.Link.PlayerChannel.Close();
        else if (!await @event.Player.IsProtocolSupportedAsync(cancellationToken))
            @event.Link.PlayerChannel.Close();

        logger.LogTrace("Stopped forwarding {Link} traffic", @event.Link);
    }

    public bool TryGetLink(IPlayer player, [NotNullWhen(true)] out ILink? link)
    {
        player = player.Unwrap();
        link = _activeLinks.FirstOrDefault(link => link.Player == player);
        return link is not null;
    }

    public bool TryGetWeakLink(IPlayer player, [NotNullWhen(true)] out ILink? link)
    {
        player = player.Unwrap();
        link = _weakLinks.FirstOrDefault(link => link.Player == player);
        return link is not null;
    }

    public bool HasLink(IPlayer player)
    {
        return _activeLinks.Any(link => link.Player == player);
    }

    private async ValueTask<ConnectionResult> ConnectCoreAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Since cancellationToken might be coming from ILink, it will be cancelled after link is destroyed.
        // All further events should have application lifetime tokens in order to know when they are forced to stop.
        // Also stopping token is used for all events triggered by link.
        cancellationToken = hostApplicationLifetime.ApplicationStopping;

        var unwrappedPlayer = player.Unwrap();

        if (TryGetLink(unwrappedPlayer, out var link))
            await link.StopAsync(cancellationToken);

        logger.LogTrace("Connecting {Player} player to a {Server} server", unwrappedPlayer, server);

        var firstConnection = unwrappedPlayer.Context.Channel is null;
        _ = await unwrappedPlayer.GetChannelBuilderAsync(cancellationToken);

        // After searching for player channel builder, player might be upgraded to another implementation, unwrap proxy again
        unwrappedPlayer = player.Unwrap();

        var playerChannel = await unwrappedPlayer.GetChannelAsync(cancellationToken);

        INetworkChannel serverChannel;

        try
        {
            serverChannel = await unwrappedPlayer.BuildServerChannelAsync(server, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning("Player {Player} cannot connect to a {Server} server because it is unavailable: {Message}", unwrappedPlayer, server, exception.Message);
            return ConnectionResult.NotConnected;
        }

        try
        {
            if (firstConnection)
                await events.ThrowAsync(new PlayerConnectedEvent(unwrappedPlayer), cancellationToken);

            link = await events.ThrowWithResultAsync(new CreateLinkEvent(unwrappedPlayer, server, playerChannel, serverChannel), cancellationToken)
                        ?? new Link(unwrappedPlayer, server, playerChannel, serverChannel, logger, events, cancellationToken);

            _weakLinks.Add(link);

            events.RegisterListeners(link);

            var side = await events.ThrowWithResultAsync(new AuthenticationStartingEvent(link, unwrappedPlayer), cancellationToken);

            if (side is AuthenticationSide.Proxy && !await unwrappedPlayer.IsProtocolSupportedAsync(cancellationToken))
            {
                logger.LogWarning("Player {Player} protocol is not supported, forcing authentication to Server side", unwrappedPlayer);
                side = AuthenticationSide.Server;
            }

            var result = await events.ThrowWithResultAsync(new AuthenticationStartedEvent(link, unwrappedPlayer, side), cancellationToken)
                ?? AuthenticationResult.NoResult;

            await events.ThrowAsync(new AuthenticationFinishedEvent(link, unwrappedPlayer, side, result), cancellationToken);

            if (!result.IsAuthenticated)
            {
                await unwrappedPlayer.KickAsync($"You are not authorized to play:\n{result.Message}", cancellationToken);
                return ConnectionResult.NotConnected;
            }
            else
            {
                using (await _lock.LockAsync(cancellationToken))
                    _activeLinks.Add(link);

                await link.StartAsync(cancellationToken);
                logger.LogInformation("Player {Player} connected to {Server} ({ProtocolVersion})", unwrappedPlayer, link.Server, unwrappedPlayer.ProtocolVersion);

                return ConnectionResult.Connected;
            }
        }
        catch (StreamException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred during authentication of {Player} to {Server}", unwrappedPlayer, server);
            await unwrappedPlayer.KickAsync("An internal error occurred during authentication", cancellationToken);

            return ConnectionResult.NotConnected;
        }
        finally
        {
            if (link is not null)
            {
                if (!_activeLinks.Contains(link))
                {
                    _weakLinks.Remove(link);
                }
            }
        }
    }
}
