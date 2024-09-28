using System.Net.Sockets;
using Nito.AsyncEx;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Links;
using Void.Proxy.API.Players;

namespace Void.Proxy.Players;

public class PlayerService : IPlayerService, IEventListener
{
    private readonly IEventService _events;
    private readonly ILinkService _links;
    private readonly AsyncLock _lock = new();
    private readonly ILogger<PlayerService> _logger;
    private readonly List<IPlayer> _players = [];
    private readonly IServiceProvider _services;

    public PlayerService(ILogger<PlayerService> logger, IServiceProvider services, ILinkService links, IEventService events)
    {
        _logger = logger;
        _services = services;
        _links = links;
        _events = events;

        events.RegisterListeners(this);
    }

    public IReadOnlyList<IPlayer> All => _players;

    public async ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default)
    {
        using var sync = await _lock.LockAsync(cancellationToken);

        _logger.LogTrace("Accepted client from {RemoteEndPoint}", client.Client.RemoteEndPoint);

        var collection = new ServiceCollection();
        _services.ForwardServices(collection);

        await _events.ThrowAsync(new PlayerConnectingEvent { Client = client, Services = collection }, cancellationToken);

        var player = new Player(client, new PlayerContext(collection.BuildServiceProvider()));

        try
        {
            _players.Add(player);
            _logger.LogInformation("Player {Player} connected", player);

            await _events.ThrowAsync(new PlayerConnectedEvent { Player = player }, cancellationToken);
            await _links.ConnectPlayerAnywhereAsync(player, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Client {RemoteEndPoint} cannot be proxied", player.RemoteEndPoint);
        }
    }

    [Subscribe]
    public async ValueTask OnPlayerDisconnected(PlayerDisconnectedEvent @event, CancellationToken cancellationToken)
    {
        using var sync = await _lock.LockAsync(cancellationToken);

        if (!_players.Remove(@event.Player))
            return;

        _logger.LogInformation("Player {Player} disconnected", @event.Player);

        await @event.Player.DisposeAsync();
    }
}