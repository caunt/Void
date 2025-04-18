﻿using Nito.AsyncEx;
using System.Diagnostics.CodeAnalysis;
using Void.Common.Network.Channels;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Authentication;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Links;

public class LinkService(ILogger<LinkService> logger, IServerService servers, IEventService events, IHostApplicationLifetime hostApplicationLifetime) : ILinkService, IEventListener
{
    private readonly List<ILink> _links = [];
    private readonly AsyncLock _lock = new();

    public async ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Looking for a server for {Player} player", player);

        var server = await events.ThrowWithResultAsync(new PlayerSearchServerEvent(player), cancellationToken)
                     ?? servers.RegisteredServers[0];

        return await ConnectAsync(player, server, cancellationToken);
    }

    public async ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Since cancellationToken might be coming from ILink, it will be cancelled after link is destroyed.
        // All further events should have application lifetime tokens in order to know when they are forced to stop.
        // Also stopping token is used for all events triggered by link.
        cancellationToken = hostApplicationLifetime.ApplicationStopping;

        if (TryGetLink(player, out var link))
            await link.StopAsync(cancellationToken);

        logger.LogTrace("Connecting {Player} player to a {Server} server", player, server);

        var firstConnection = player.Context.Channel is null;
        var playerChannel = await player.GetChannelAsync(cancellationToken);

        INetworkChannel serverChannel;

        try
        {
            serverChannel = await player.BuildServerChannelAsync(server, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning("Player {Player} cannot connect to a {Server} server because it is unavailable: {Message}", player, server, exception.Message);
            logger.LogTrace("Player {Player} cannot connect to a {Server} server because it is unavailable:\n{Exception}", player, server, exception);

            await player.KickAsync($"Server {server} is unavailable", cancellationToken);
            return ConnectionResult.NotConnected;
        }

        if (firstConnection)
            await events.ThrowAsync(new PlayerConnectedEvent(player), cancellationToken);

        link = await events.ThrowWithResultAsync(new CreateLinkEvent(player, server, playerChannel, serverChannel), cancellationToken)
                   ?? new Link(player, server, playerChannel, serverChannel, logger, events, cancellationToken);

        using (await _lock.LockAsync(cancellationToken))
            _links.Add(link);

        events.RegisterListeners(link);

        var side = await events.ThrowWithResultAsync(new AuthenticationStartingEvent(link), cancellationToken);

        if (side is AuthenticationSide.Proxy && !await player.IsProtocolSupportedAsync(cancellationToken))
        {
            logger.LogWarning("Player {Player} protocol is not supported, forcing authentication to Server side", player);
            side = AuthenticationSide.Server;
        }

        var result = await events.ThrowWithResultAsync(new AuthenticationStartedEvent(link, side), cancellationToken);

        if (result is AuthenticationResult.NoResult)
            throw new InvalidOperationException($"No {nameof(AuthenticationResult)} provided for {link}");

        await events.ThrowAsync(new AuthenticationFinishedEvent(link, side, result), cancellationToken);

        if (result is AuthenticationResult.NotAuthenticatedPlayer or AuthenticationResult.NotAuthenticatedServer)
        {
            await player.KickAsync("You are not authorized to play", cancellationToken);
            return ConnectionResult.NotConnected;
        }
        else
        {
            await link.StartAsync(cancellationToken);
            logger.LogInformation("Player {Player} connected to {Server}", player, link.Server);

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

        logger.LogTrace("Stopped forwarding {Link} traffic", @event.Link);
    }

    public bool TryGetLink(IPlayer player, [NotNullWhen(true)] out ILink? link)
    {
        link = _links.FirstOrDefault(link => link.Player == player);
        return link is not null;
    }
}
