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
    private readonly List<ILink> _links =[];
    
    private readonly ILogger<LinkService> _logger;
    private readonly IServerService _servers;
    private readonly IEventService _events;

    public LinkService(ILogger<LinkService> logger, IServerService servers, IEventService events)
    {
        _logger = logger;
        _servers = servers;
        _events = events;
        
        events.RegisterListeners(this);
    }

    public async ValueTask ConnectPlayerAnywhereAsync(IPlayer player)
    {
        var server = _servers.RegisteredServers.First();

        var playerChannel = await player.GetChannelAsync();
        var serverChannel = await player.BuildServerChannelAsync(server);

        var link = await CreateLinkAsync(player, server, playerChannel, serverChannel);
        _links.Add(link);

        _logger.LogInformation("Started forwarding {Link} traffic", link);
    }

    [Subscribe]
    public async ValueTask OnStopLinkEvent(StopLinkEvent @event)
    {
        if (!_links.Remove(@event.Link))
            return;

        await using var _ = @event.Link;

        if (@event.Link.IsAlive)
            throw new Exception($"Link {@event.Link} is still alive");

        _logger.LogInformation("Stopped forwarding {Link} traffic", @event.Link);
    }

    private async ValueTask<ILink> CreateLinkAsync(IPlayer player, IServer server, IMinecraftChannel playerChannel, IMinecraftChannel serverChannel)
    {
        var @event = new CreateLinkEvent
        {
            Player = player,
            Server = server,
            PlayerChannel = playerChannel,
            ServerChannel = serverChannel
        };

        await _events.ThrowAsync(@event);

        var link = @event.Result ?? new Link(player, server, playerChannel, serverChannel, _events);
        
        await _events.ThrowAsync(new StartLinkEvent { Link = link });

        return link;
    }
}