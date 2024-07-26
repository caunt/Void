using Nito.AsyncEx;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Links;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network.IO.Channels;
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

    public async ValueTask ConnectPlayerAnywhereAsync(IPlayer player)
    {
        using var _ = await _lock.LockAsync();

        var server = _servers.RegisteredServers.First();

        var playerChannel = await player.GetChannelAsync();
        var serverChannel = await player.BuildServerChannelAsync(server);

        var link = await CreateLinkAsync(player, server, playerChannel, serverChannel);
        await _events.ThrowAsync(new LinkStartingEvent { Link = link });

        _events.RegisterListeners(link);
        _links.Add(link);

        _logger.LogInformation("Started forwarding {Link} traffic", link);
        await _events.ThrowAsync(new LinkStartedEvent { Link = link });
    }

    [Subscribe]
    public async ValueTask OnStopLinkEvent(LinkStoppingEvent @event)
    {
        using var _ = await _lock.LockAsync();

        if (!_links.Remove(@event.Link))
            return;

        await @event.Link.DisposeAsync();

        if (@event.Link.IsAlive)
            throw new Exception($"Link {@event.Link} is still alive");

        _logger.LogInformation("Stopped forwarding {Link} traffic", @event.Link);
        await _events.ThrowAsync(new LinkStoppedEvent { Link = @event.Link });
    }

    private async ValueTask<ILink> CreateLinkAsync(IPlayer player, IServer server, IMinecraftChannel playerChannel, IMinecraftChannel serverChannel)
    {
        var link = await _events.ThrowWithResultAsync(new CreateLinkEvent
        {
            Player = player,
            Server = server, 
            PlayerChannel = playerChannel, 
            ServerChannel = serverChannel
        }) ?? new Link(player, server, playerChannel, serverChannel, _events);

        return link;
    }

    #region Injected

    private readonly IEventService _events;
    private readonly ILogger<LinkService> _logger;
    private readonly IServerService _servers;

    #endregion Injected
}