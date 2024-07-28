using Nito.AsyncEx;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Links;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Links;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.Links;

public class LinkService : ILinkService, IEventListener
{
    private readonly AsyncLock _lock = new();
    private readonly List<ILink> _links = [];

    public LinkService(ILogger<LinkService> logger, IServerService servers, IEventService events)
    {
        _logger = logger;
        _servers = servers;
        _events = events;

        events.RegisterListeners(this);
    }

    public async ValueTask ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        var server = await _events.ThrowWithResultAsync(new PlayerSearchServerEvent { Player = player }, cancellationToken) ?? _servers.RegisteredServers.First();
        await ConnectAsync(player, server, cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnStopLinkEvent(LinkStoppingEvent @event, CancellationToken cancellationToken)
    {
        using var sync = await _lock.LockAsync();

        if (!_links.Remove(@event.Link))
            return;

        await @event.Link.DisposeAsync();

        if (@event.Link.IsAlive)
            throw new Exception($"Link {@event.Link} is still alive");

        _logger.LogInformation("Stopped forwarding {Link} traffic", @event.Link);
        await _events.ThrowAsync(new LinkStoppedEvent { Link = @event.Link }, cancellationToken);

        if (@event.Link.Player.Client.Connected)
            _ = ConnectPlayerAnywhereAsync(@event.Link.Player, cancellationToken)
                .CatchExceptions(); // release async lock
        else
            _ = _events.ThrowAsync(new PlayerDisconnectedEvent { Player = @event.Link.Player }, cancellationToken)
                .CatchExceptions(); // sometimes deadlocks with PlayerService synchronization, so must release async lock
    }

    private async ValueTask ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        using var sync = await _lock.LockAsync(cancellationToken);

        var playerChannel = await player.GetChannelAsync(cancellationToken);
        var serverChannel = await player.BuildServerChannelAsync(server, cancellationToken);

        var link = await _events.ThrowWithResultAsync(new CreateLinkEvent
            {
                Player = player,
                Server = server,
                PlayerChannel = playerChannel,
                ServerChannel = serverChannel
            },
            cancellationToken) ?? new Link(player, server, playerChannel, serverChannel);
        await _events.ThrowAsync(new LinkStartingEvent { Link = link }, cancellationToken);

        _events.RegisterListeners(link);
        _links.Add(link);

        _logger.LogInformation("Started forwarding {Link} traffic", link);
        await _events.ThrowAsync(new LinkStartedEvent { Link = link }, cancellationToken);
    }

    #region Injected

    private readonly IEventService _events;
    private readonly ILogger<LinkService> _logger;
    private readonly IServerService _servers;

    #endregion Injected
}