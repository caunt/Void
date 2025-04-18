using Nito.AsyncEx;
using System.Diagnostics;
using System.Net.Sockets;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Settings;
using Void.Proxy.Players.Contexts;

namespace Void.Proxy.Players;

public class PlayerService(ILogger<PlayerService> logger, IServiceProvider services, ILinkService links, IEventService events, ISettings settings) : IPlayerService, IEventListener
{
    private readonly AsyncLock _lock = new();
    private readonly List<IPlayer> _players = [];

    public IReadOnlyList<IPlayer> All => _players;

    public async ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Accepted client from {RemoteEndPoint}", client.Client.RemoteEndPoint);

        await events.ThrowAsync(new PlayerConnectingEvent(client), cancellationToken);
        var player = new Player(client) { Context = new PlayerContext(services.CreateAsyncScope()) };

        try
        {
            using (var sync = await _lock.LockAsync(cancellationToken))
                _players.Add(player);

            logger.LogTrace("Player {Player} connecting", player);
            var result = await links.ConnectPlayerAnywhereAsync(player, cancellationToken);

            if (!links.TryGetLink(player, out var link))
                logger.LogWarning("Player {Player} failed to connect", player);
        }
        catch (Exception exception)
        {
            if (exception is not StreamClosedException)
                logger.LogError(exception, "Client {RemoteEndPoint} cannot be proxied", player.RemoteEndPoint);

            // just in case
            await events.ThrowAsync(new PlayerDisconnectedEvent(player), cancellationToken);
        }
    }

    [Subscribe]
    public async ValueTask OnLinkStopped(LinkStoppedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Link.PlayerChannel.IsAlive)
            await links.ConnectPlayerAnywhereAsync(@event.Link.Player, cancellationToken);
        else
            await events.ThrowAsync(new PlayerDisconnectedEvent(@event.Link.Player), cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnPlayerDisconnected(PlayerDisconnectedEvent @event, CancellationToken cancellationToken)
    {
        using (var sync = await _lock.LockAsync(cancellationToken))
            if (!_players.Remove(@event.Player))
                return;

        logger.LogInformation("Player {Player} disconnected", @event.Player);

        await @event.Player.DisposeAsync();
    }

    public async ValueTask KickPlayerAsync(IPlayer player, string? text = null, CancellationToken cancellationToken = default)
    {
        await KickPlayerAsync(player, new PlayerKickEvent(player, text), cancellationToken);
    }

    public async ValueTask KickPlayerAsync(IPlayer player, PlayerKickEvent playerKickEvent, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Kicking player {Player}", player);

        if (!All.Contains(player))
            return;

        var channel = await player.GetChannelAsync(cancellationToken);

        if (links.TryGetLink(player, out var link))
        {
            link.PlayerChannel.TryPause();
            link.ServerChannel.TryPause();
        }

        var kicked = await events.ThrowWithResultAsync(playerKickEvent, cancellationToken);

        if (kicked)
        {
            var timestamp = Stopwatch.GetTimestamp();

            while (channel.IsAlive)
            {
                if (Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds > settings.KickTimeout)
                    break;

                if (cancellationToken.IsCancellationRequested)
                    break;

                await Task.Delay(100, cancellationToken);
            }

            if (channel.IsAlive)
                logger.LogWarning("Player {Player} didn't handle graceful kick in {Timeout}ms", player, settings.KickTimeout);
        }

        if (link is not null)
        {
            await link.PlayerChannel.FlushAsync(cancellationToken);
            await link.ServerChannel.FlushAsync(cancellationToken);

            link.PlayerChannel.Close();
            link.ServerChannel.Close();
        }
        else
        {
            await channel.FlushAsync(cancellationToken);
            player.Client.Close();
        }

        if (link is not { IsAlive: true })
        {
            // since ILink is not running and will not execute link stopped event, trigger player disconnected event here
            await events.ThrowAsync(new PlayerDisconnectedEvent(player), cancellationToken);
        }
        else
        {
            link?.PlayerChannel.TryResume();
            link?.ServerChannel.TryResume();
        }
    }
}
