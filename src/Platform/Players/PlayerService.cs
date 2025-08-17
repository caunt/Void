using System.Diagnostics;
using System.Net.Sockets;
using Nito.AsyncEx;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Api.Settings;
using Void.Proxy.Players.Extensions;

namespace Void.Proxy.Players;

public class PlayerService(ILogger<PlayerService> logger, IDependencyService dependencies, ILinkService links, IEventService events, ISettings settings) : IPlayerService, IEventListener
{
    private readonly AsyncLock _lock = new();
    private readonly List<PlayerProxy> _players = [];

    public IEnumerable<IPlayer> All => _players.Select(proxy => proxy.Source);

    public void ForEach(Action<IPlayer> action)
    {
        for (var i = _players.Count - 1; i >= 0; i--)
            action(_players[i].Unwrap());
    }

    public async ValueTask ForEachAsync(Func<IPlayer, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        for (var i = _players.Count - 1; i >= 0; i--)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await action(_players[i].Unwrap(), cancellationToken);
        }
    }

    public async ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Accepted client from {RemoteEndPoint}", client.Client.RemoteEndPoint);

        var player = new PlayerProxy(await events.ThrowWithResultAsync(new PlayerConnectingEvent(client, dependencies.CreatePlayerComposite), cancellationToken) ??
            throw new InvalidOperationException("Player is not instantiated"));

        dependencies.ActivatePlayerContext(player.Context);

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
            if (exception is StreamClosedException)
                return;

            logger.LogError(exception, "Client {RemoteEndPoint} cannot be proxied", player.RemoteEndPoint);
        }
    }

    public async ValueTask UpgradeAsync(IPlayer player, IPlayer upgradedPlayer, CancellationToken cancellationToken)
    {
        using var sync = await _lock.LockAsync(cancellationToken);

        var proxy = _players.FirstOrDefault(proxy => proxy == player || proxy.Source == player) ??
            throw new InvalidOperationException($"Player {player} not found");

        proxy.Replace(upgradedPlayer);
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
                logger.LogWarning("Plugins didn't handle graceful {Player} player kick in {Timeout}ms", player, settings.KickTimeout);
        }

        link?.PlayerChannel.TryResume();
        link?.ServerChannel.TryResume();

        await channel.FlushAsync(cancellationToken);

        if (link is not null)
        {
            await link.PlayerChannel.FlushAsync(cancellationToken);
            await link.ServerChannel.FlushAsync(cancellationToken);
        }

        link?.PlayerChannel.Close();
        link?.ServerChannel.Close();

        player.Client.Close();

        await events.ThrowAsync(new PlayerDisconnectedEvent(player), cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnLinkStopped(LinkStoppedEvent @event, CancellationToken cancellationToken)
    {
        // All other reasons should throw player disconnected event themselves
        if (@event.Reason is LinkStopReason.PlayerDisconnected)
        {
            await events.ThrowAsync(new PlayerDisconnectedEvent(@event.Player), cancellationToken);
            return;
        }

        if (@event.Reason is LinkStopReason.Requested || !@event.Link.PlayerChannel.IsAlive)
            return;

        await links.ConnectPlayerAnywhereAsync(@event.Player, cancellationToken);
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnPlayerDisconnected(PlayerDisconnectedEvent @event, CancellationToken cancellationToken)
    {
        using (var sync = await _lock.LockAsync(cancellationToken))
        {
            var count = _players.RemoveAll(proxy => proxy == @event.Player || proxy.Source == @event.Player);

            if (count is 0)
                return;

            if (count > 1)
                logger.LogWarning("Multiple players disconnected in one event");
        }

        logger.LogInformation("Player {Player} disconnected", @event.Player);

        dependencies.DisposePlayerContext(@event.Player.Context);
        await @event.Player.DisposeAsync();
    }
}
