﻿using Nito.AsyncEx;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Authentication;
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
    private readonly IEventService _events;
    private readonly List<ILink> _links = [];
    private readonly AsyncLock _lock = new();
    private readonly ILogger<LinkService> _logger;
    private readonly IServerService _servers;

    public LinkService(ILogger<LinkService> logger, IServerService servers, IEventService events)
    {
        _logger = logger;
        _servers = servers;
        _events = events;

        events.RegisterListeners(this);
    }

    public async ValueTask ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Looking for a server for {Player} player", player);
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
            _ = ConnectPlayerAnywhereAsync(@event.Link.Player, cancellationToken).CatchExceptions(); // release async lock
        else
            _ = _events.ThrowAsync(new PlayerDisconnectedEvent { Player = @event.Link.Player }, cancellationToken).CatchExceptions(); // sometimes deadlocks with PlayerService synchronization, so must release async lock
    }

    private async ValueTask ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        using var sync = await _lock.LockAsync(cancellationToken);

        _logger.LogTrace("Connecting {Player} player to a {Server} server", player, server);

        var firstConnection = player.Context.Channel is null;

        var playerChannel = await player.GetChannelAsync(cancellationToken);
        var serverChannel = await player.BuildServerChannelAsync(server, cancellationToken);

        if (firstConnection)
            await _events.ThrowAsync(new PlayerConnectedEvent { Player = player }, cancellationToken);

        var link = await _events.ThrowWithResultAsync(new CreateLinkEvent
        {
            Player = player,
            Server = server,
            PlayerChannel = playerChannel,
            ServerChannel = serverChannel
        }, cancellationToken) ?? new Link(player, server, playerChannel, serverChannel, _logger, _events);

        _links.Add(link);

        var side = await _events.ThrowWithResultAsync(new AuthenticationStartingEvent { Link = link }, cancellationToken);

        if (side is AuthenticationSide.Proxy && !await player.IsProtocolSupportedAsync(cancellationToken))
        {
            _logger.LogWarning("Player {Player} protocol is not supported, forcing authentication to Server side", player);
            side = AuthenticationSide.Server;
        }

        await _events.ThrowAsync(new AuthenticationStartedEvent { Link = link, Side = side }, cancellationToken);
        await link.StartAsync(cancellationToken);
    }
}