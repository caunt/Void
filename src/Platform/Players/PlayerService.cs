using Nito.AsyncEx;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Settings;
using Void.Proxy.Players.Contexts;

namespace Void.Proxy.Players;

public class PlayerService : IPlayerService, IEventListener
{
    private readonly IEventService _events;
    private readonly ILinkService _links;
    private readonly AsyncLock _lock = new();
    private readonly ILogger<PlayerService> _logger;
    private readonly List<IPlayer> _players = [];
    private readonly IServiceProvider _services;
    private readonly ISettings _settings;

    public PlayerService(ILogger<PlayerService> logger, IServiceProvider services, ILinkService links, IEventService events, ISettings settings)
    {
        _logger = logger;
        _services = services;
        _links = links;
        _events = events;
        _settings = settings;

        events.RegisterListeners(this);
    }

    public IReadOnlyList<IPlayer> All => _players;

    public async ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Accepted client from {RemoteEndPoint}", client.Client.RemoteEndPoint);

        var collection = new ServiceCollection();
        _services.ForwardServices(collection);

        await _events.ThrowAsync(new PlayerConnectingEvent(client, collection), cancellationToken);

        var player = new Player(client);
        collection.AddSingleton<IPlayer>(player);
        player.Context = new PlayerContext(collection.BuildServiceProvider());

        try
        {
            using (var sync = await _lock.LockAsync(cancellationToken))
                _players.Add(player);

            _logger.LogTrace("Player {Player} connecting", player);
            var result = await _links.ConnectPlayerAnywhereAsync(player, cancellationToken);

            if (_links.TryGetLink(player, out var link))
                _logger.LogInformation("Player {Player} connected to {Server}", player, link.Server);
            else
                _logger.LogWarning("Player {Player} failed to connect", player);
        }
        catch (Exception exception)
        {
            if (exception is not EndOfStreamException)
                _logger.LogError(exception, "Client {RemoteEndPoint} cannot be proxied", player.RemoteEndPoint);

            // just in case
            await _events.ThrowAsync(new PlayerDisconnectedEvent(player), cancellationToken);
        }
    }

    public async ValueTask KickPlayerAsync(IPlayer player, Component? reason = null, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Kicking player {Player}", player);

        if (!_players.Contains(player))
            return;

        var channel = await player.GetChannelAsync(cancellationToken);

        if (_links.TryGetLink(player, out var link))
        {
            link.PlayerChannel.TryPause();
            link.ServerChannel.TryPause();
        }

        var kicked = await _events.ThrowWithResultAsync(new PlayerKickEvent(player, reason), cancellationToken);

        if (kicked)
        {
            var timestamp = Stopwatch.GetTimestamp();

            while (channel.IsAlive)
            {
                if (Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds > _settings.KickTimeout)
                    break;

                if (cancellationToken.IsCancellationRequested)
                    break;

                await Task.Delay(100, cancellationToken);
            }

            if (channel.IsAlive)
                _logger.LogWarning("Player {Player} didn't handle graceful kick in {Timeout}ms", player, _settings.KickTimeout);
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
            await _events.ThrowAsync(new PlayerDisconnectedEvent(player), cancellationToken);
        }
        else
        {
            link?.PlayerChannel.TryResume();
            link?.ServerChannel.TryResume();
        }
    }

    [Subscribe]
    public async ValueTask OnLinkStopped(LinkStoppedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Link.PlayerChannel.IsAlive)
            await _links.ConnectPlayerAnywhereAsync(@event.Link.Player, cancellationToken);
        else
            await _events.ThrowAsync(new PlayerDisconnectedEvent(@event.Link.Player), cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnPlayerDisconnected(PlayerDisconnectedEvent @event, CancellationToken cancellationToken)
    {
        using (var sync = await _lock.LockAsync(cancellationToken))
            if (!_players.Remove(@event.Player))
                return;

        _logger.LogInformation("Player {Player} disconnected", @event.Player);

        await @event.Player.DisposeAsync();
    }

    public bool TryGetByName(string name, [NotNullWhen(true)] out IPlayer? player)
    {
        player = _players.FirstOrDefault(p => p.Profile?.Username.Equals(name, StringComparison.InvariantCultureIgnoreCase) ?? false);
        return player is not null;
    }
}