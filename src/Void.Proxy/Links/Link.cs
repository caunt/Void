﻿using Nito.AsyncEx;
using Void.Proxy.API.Events.Links;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Streams.Transparent;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.Links;

public class Link : ILink
{
    private CancellationTokenSource _ctsPlayerToServer;
    private CancellationTokenSource _ctsPlayerToServerForce;
    private CancellationTokenSource _ctsServerToPlayer;
    private CancellationTokenSource _ctsServerToPlayerForce;
    private readonly IEventService _events;
    private readonly AsyncLock _lock;
    private readonly ILogger<Link> _logger;
    private Task _playerToServerTask;
    private Task _serverToPlayerTask;

    public Link(IPlayer player, IServer server, IMinecraftChannel playerChannel, IMinecraftChannel serverChannel)
    {
        Player = player;
        Server = server;
        PlayerChannel = playerChannel;
        ServerChannel = serverChannel;

        _logger = player.Scope.ServiceProvider.GetRequiredService<ILogger<Link>>();
        _events = player.Scope.ServiceProvider.GetRequiredService<IEventService>();
        _lock = new AsyncLock();

        _ctsPlayerToServer = new CancellationTokenSource();
        _ctsPlayerToServerForce = new CancellationTokenSource();
        _ctsServerToPlayer = new CancellationTokenSource();
        _ctsServerToPlayerForce = new CancellationTokenSource();

        _playerToServerTask = ExecuteAsync(PlayerChannel, ServerChannel, Direction.Serverbound, _ctsPlayerToServer.Token, _ctsPlayerToServerForce.Token);
        _serverToPlayerTask = ExecuteAsync(ServerChannel, PlayerChannel, Direction.Clientbound, _ctsServerToPlayer.Token, _ctsServerToPlayerForce.Token);
    }

    public IPlayer Player { get; init; }
    public IServer Server { get; init; }
    public IMinecraftChannel PlayerChannel { get; init; }
    public IMinecraftChannel ServerChannel { get; init; }

    public bool IsAlive => _playerToServerTask.Status == TaskStatus.Running && _serverToPlayerTask.Status == TaskStatus.Running;
    public bool IsRestarting { get; private set; }

    public async ValueTask RestartAsync(CancellationToken cancellationToken = default)
    {
        if (IsRestarting)
        {
            _logger.LogWarning("Link {Link} is already restarting", this);
            return;
        }

        _logger.LogDebug("Link {Link} is restarting", this);

        IsRestarting = true;

        await _ctsServerToPlayer.CancelAsync();
        _ctsServerToPlayer = new CancellationTokenSource();

        if (await WaitWithTimeout(_serverToPlayerTask))
        {
            _logger.LogInformation("Timed out waiting Server {Server} disconnection from Player {Player} manually, closing forcefully (Restart)", Server, Player);
            await _ctsServerToPlayerForce.CancelAsync();
            _ctsServerToPlayerForce = new CancellationTokenSource();

            if (await WaitWithTimeout(_serverToPlayerTask))
                throw new Exception($"Cannot dispose Link {this} (player=>server) (Restart)");
        }

        await _ctsPlayerToServer.CancelAsync();
        _ctsPlayerToServer = new CancellationTokenSource();

        if (await WaitWithTimeout(_playerToServerTask))
        {
            _logger.LogInformation("Timed out waiting Player {Player} disconnection from Server {Server} manually, closing forcefully (Restart)", Player, Server);
            await _ctsPlayerToServerForce.CancelAsync();
            _ctsPlayerToServerForce = new CancellationTokenSource();

            if (await WaitWithTimeout(_playerToServerTask))
                throw new Exception($"Cannot dispose Link {this} (server=>player) (Restart)");
        }

        IsRestarting = false;

        _playerToServerTask = ExecuteAsync(PlayerChannel, ServerChannel, Direction.Serverbound, _ctsPlayerToServer.Token, _ctsPlayerToServerForce.Token);
        _serverToPlayerTask = ExecuteAsync(ServerChannel, PlayerChannel, Direction.Clientbound, _ctsServerToPlayer.Token, _ctsServerToPlayerForce.Token);

        _logger.LogDebug("Link {Link} successfully restarted", this);
    }

    public async ValueTask DisposeAsync()
    {
        // if the player does not support redirections, that's the end for him
        if (PlayerChannel.Head is MinecraftTransparentMessageStream)
        {
            PlayerChannel.Close();
            await PlayerChannel.DisposeAsync();
        }

        ServerChannel.Close();
        await ServerChannel.DisposeAsync();

        if (await WaitWithTimeout(_serverToPlayerTask))
        {
            _logger.LogInformation("Timed out waiting Server {Server} disconnection from Player {Player}, closing manually", Server, Player);
            await _ctsServerToPlayer.CancelAsync();

            if (await WaitWithTimeout(_serverToPlayerTask))
            {
                _logger.LogInformation("Timed out waiting Server {Server} disconnection from Player {Player} manually, closing forcefully", Server, Player);
                await _ctsServerToPlayerForce.CancelAsync();

                if (await WaitWithTimeout(_serverToPlayerTask))
                    throw new Exception($"Cannot dispose Link {this} (player=>server)");
            }
        }

        if (await WaitWithTimeout(_playerToServerTask))
        {
            _logger.LogInformation("Timed out waiting Player {Player} disconnection from Server {Server}, closing manually", Player, Server);
            await _ctsPlayerToServer.CancelAsync();

            if (await WaitWithTimeout(_playerToServerTask))
            {
                _logger.LogInformation("Timed out waiting Player {Player} disconnection from Server {Server} manually, closing forcefully", Player, Server);
                await _ctsPlayerToServerForce.CancelAsync();

                if (await WaitWithTimeout(_playerToServerTask))
                    throw new Exception($"Cannot dispose Link {this} (server=>player)");
            }
        }

        await Task.WhenAll(_playerToServerTask, _serverToPlayerTask);
    }

    public override string ToString()
    {
        return Player + " <=> " + Server;
    }

    protected async Task ExecuteAsync(IMinecraftChannel sourceChannel, IMinecraftChannel destinationChannel, Direction direction, CancellationToken cancellationToken, CancellationToken forceCancellationToken)
    {
        await Task.Yield();

        // not sure if reading should be cancelled with grace token
        var readingCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, forceCancellationToken);

        try
        {
            while (sourceChannel is { CanRead: true, CanWrite: true } && destinationChannel is { CanRead: true, CanWrite: true })
            {
                if (cancellationToken.IsCancellationRequested || forceCancellationToken.IsCancellationRequested)
                    break;

                using var message = await sourceChannel.ReadMessageAsync(readingCancellationTokenSource.Token);

                var cancelled = await _events.ThrowWithResultAsync(new MessageReceivedEvent
                {
                    From = (Side)direction,
                    To = direction == Direction.Serverbound ? Side.Server : Side.Client,
                    Direction = direction,
                    Message = message,
                    Link = this
                }, cancellationToken);

                if (cancelled)
                    continue;

                using var _ = await _lock.LockAsync();
                await destinationChannel.WriteMessageAsync(message, forceCancellationToken);

                await _events.ThrowAsync(new MessageSentEvent
                {
                    From = (Side)direction,
                    To = direction == Direction.Serverbound ? Side.Server : Side.Client,
                    Direction = direction,
                    Message = message,
                    Link = this
                }, cancellationToken);
            }
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or TaskCanceledException or OperationCanceledException or ObjectDisposedException)
        {
            if (IsRestarting)
                return;

            if (direction is not Direction.Serverbound)
                return;

            PlayerChannel.Close();
            await PlayerChannel.DisposeAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled {Direction} exception from {Player}", direction, Player);
        }
        finally
        {
            await PlayerChannel.FlushAsync(forceCancellationToken);
            await ServerChannel.FlushAsync(forceCancellationToken);

            if (!IsRestarting)
                _ = _events.ThrowAsync(new LinkStoppingEvent { Link = this }, forceCancellationToken).CatchExceptions();
        }
    }

    private static async Task<bool> WaitWithTimeout(Task task, int milliseconds = 5000)
    {
        var timeout = Task.Delay(milliseconds);
        return await Task.WhenAny(timeout, task) == timeout;
    }
}