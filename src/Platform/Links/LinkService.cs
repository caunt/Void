using Nito.AsyncEx;
using System.Diagnostics.CodeAnalysis;
using Void.Common;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Authentication;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Links;

public class LinkService : ILinkService, IEventListener
{
    private readonly IEventService _events;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly List<ILink> _links = [];
    private readonly AsyncLock _lock = new();
    private readonly ILogger<LinkService> _logger;
    private readonly IServerService _servers;

    public LinkService(ILogger<LinkService> logger, IServerService servers, IEventService events, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _servers = servers;
        _events = events;
        _hostApplicationLifetime = hostApplicationLifetime;

        events.RegisterListeners(this);
    }

    public async ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Looking for a server for {Player} player", player);

        var server = await _events.ThrowWithResultAsync(new PlayerSearchServerEvent(player), cancellationToken)
                     ?? _servers.RegisteredServers[0];

        return await ConnectAsync(player, server, cancellationToken);
    }

    public async ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Since cancellationToken might be coming from ILink, it will be cancelled after link is destroyed.
        // All further events should have application lifetime tokens in order to know when they are forced to stop.
        cancellationToken = _hostApplicationLifetime.ApplicationStopping;

        if (TryGetLink(player, out var link))
            await link.StopAsync(cancellationToken);

        _logger.LogTrace("Connecting {Player} player to a {Server} server", player, server);

        var firstConnection = player.Context.Channel is null;
        var playerChannel = await player.GetChannelAsync(cancellationToken);

        IMinecraftChannel serverChannel;

        try
        {
            serverChannel = await player.BuildServerChannelAsync(server, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning("Player {Player} cannot connect to a {Server} server because it is unavailable: {Message}", player, server, exception.Message);
            _logger.LogTrace("Player {Player} cannot connect to a {Server} server because it is unavailable:\n{Exception}", player, server, exception);

            await player.KickAsync($"Server {server} is unavailable", cancellationToken);
            return ConnectionResult.NotConnected;
        }

        if (firstConnection)
            await _events.ThrowAsync(new PlayerConnectedEvent(player), cancellationToken);

        link = await _events.ThrowWithResultAsync(new CreateLinkEvent(player, server, playerChannel, serverChannel), cancellationToken)
                   ?? new Link(player, server, playerChannel, serverChannel, _logger, _events);

        using (await _lock.LockAsync(cancellationToken))
            _links.Add(link);

        _events.RegisterListeners(link);

        var side = await _events.ThrowWithResultAsync(new AuthenticationStartingEvent(link), cancellationToken);

        if (side is AuthenticationSide.Proxy && !await player.IsProtocolSupportedAsync(cancellationToken))
        {
            _logger.LogWarning("Player {Player} protocol is not supported, forcing authentication to Server side", player);
            side = AuthenticationSide.Server;
        }

        var result = await _events.ThrowWithResultAsync(new AuthenticationStartedEvent(link, side), cancellationToken);

        if (result is AuthenticationResult.NoResult)
            throw new InvalidOperationException($"No {nameof(AuthenticationResult)} provided for {link}");

        await _events.ThrowAsync(new AuthenticationFinishedEvent(link, side, result), cancellationToken);

        if (result is AuthenticationResult.NotAuthenticatedPlayer or AuthenticationResult.NotAuthenticatedServer)
        {
            await player.KickAsync("You are not authorized to play", cancellationToken);
            return ConnectionResult.NotConnected;
        }
        else
        {
            await link.StartAsync(cancellationToken);
            return ConnectionResult.Connected;
        }
    }

    [Subscribe(PostOrder.First)]
    public async ValueTask OnLinkStopped(LinkStoppedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Link.IsAlive)
            throw new Exception($"Link {@event.Link} is still alive");

        using (await _lock.LockAsync(cancellationToken))
        {
            if (!_links.Remove(@event.Link))
                throw new InvalidOperationException($"Link {@event.Link} is not found");
        }

        // IServer channel is no longed needed
        @event.Link.ServerChannel.Close();

        if (!@event.Link.PlayerChannel.IsAlive)
            @event.Link.PlayerChannel.Close();
        else if (!await @event.Link.Player.IsProtocolSupportedAsync(cancellationToken))
            @event.Link.PlayerChannel.Close();

        _logger.LogTrace("Stopped forwarding {Link} traffic", @event.Link);
    }

    public bool TryGetLink(IPlayer player, [NotNullWhen(true)] out ILink? link)
    {
        link = _links.FirstOrDefault(link => link.Player == player);
        return link is not null;
    }
}
