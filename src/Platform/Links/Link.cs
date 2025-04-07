using Void.Common;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Links;

public class Link(IPlayer player, IServer server, IMinecraftChannel playerChannel, IMinecraftChannel serverChannel, ILogger logger, IEventService events) : ILink
{
    private readonly CancellationTokenSource _ctsPlayerToServer = new();
    private readonly CancellationTokenSource _ctsPlayerToServerForce = new();
    private readonly CancellationTokenSource _ctsServerToPlayer = new();
    private readonly CancellationTokenSource _ctsServerToPlayerForce = new();

    private Task? _playerToServerTask;
    private Task? _serverToPlayerTask;
    private bool _playerToServerTaskDisposed;
    private bool _serverToPlayerTaskDisposed;
    private bool _stopping;
    private bool _stopped;

    public IPlayer Player { get; init; } = player;
    public IServer Server { get; init; } = server;
    public IMinecraftChannel PlayerChannel { get; init; } = playerChannel;
    public IMinecraftChannel ServerChannel { get; init; } = serverChannel;

    public bool IsAlive => this is { _playerToServerTask.IsCompleted: false, _serverToPlayerTask.IsCompleted: false };

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (this is { _playerToServerTask: not null } or { _serverToPlayerTask: not null })
            throw new InvalidOperationException("Link was already started");

        await events.ThrowAsync(new LinkStartingEvent(this), cancellationToken);

        _playerToServerTask = ExecuteAsync(PlayerChannel, ServerChannel, Direction.Serverbound, _ctsPlayerToServer.Token, _ctsPlayerToServerForce.Token);
        _serverToPlayerTask = ExecuteAsync(ServerChannel, PlayerChannel, Direction.Clientbound, _ctsServerToPlayer.Token, _ctsServerToPlayerForce.Token);

        logger.LogTrace("Started forwarding {Link} traffic", this);
        await events.ThrowAsync(new LinkStartedEvent(this), cancellationToken);
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (this is { _playerToServerTask: null } or { _serverToPlayerTask: null })
            throw new InvalidOperationException("Link is not started");

        await DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeClientboundAsync();
        await DisposeServerboundAsync();

        GC.SuppressFinalize(this);
    }

    protected async ValueTask DisposeServerboundAsync()
    {
        if (!await Player.IsProtocolSupportedAsync())
        {
            // PlayerChannel.Close();
            // await PlayerChannel.DisposeAsync();
        }

        if (_playerToServerTaskDisposed)
            return;

        if (_playerToServerTask is not null)
        {
            await _ctsPlayerToServer.CancelAsync();

            if (await WaitWithTimeout(_playerToServerTask))
            {
                logger.LogTrace("Timed out waiting Player {Player} disconnection from Server {Server} manually, closing forcefully", Player, Server);
                await _ctsPlayerToServerForce.CancelAsync();

                if (await WaitWithTimeout(_playerToServerTask))
                    throw new Exception($"Cannot dispose Link {this} (server=>player)");
            }

            if (_playerToServerTask.IsCanceled)
                return;

            await _playerToServerTask;
        }
    }

    protected async ValueTask DisposeClientboundAsync()
    {
        // ServerChannel.Close();
        // await ServerChannel.DisposeAsync();

        if (_serverToPlayerTaskDisposed)
            return;

        if (_serverToPlayerTask is not null)
        {
            await _ctsServerToPlayer.CancelAsync();

            if (await WaitWithTimeout(_serverToPlayerTask))
            {
                logger.LogTrace("Timed out waiting Server {Server} disconnection from Player {Player} manually, closing forcefully", Server, Player);
                await _ctsServerToPlayerForce.CancelAsync();

                if (await WaitWithTimeout(_serverToPlayerTask))
                    throw new Exception($"Cannot dispose Link {this} (player=>server)");
            }

            if (_serverToPlayerTask.IsCanceled)
                return;

            await _serverToPlayerTask;
        }
    }

    protected async Task ExecuteAsync(IMinecraftChannel sourceChannel, IMinecraftChannel destinationChannel, Direction direction, CancellationToken cancellationToken, CancellationToken forceCancellationToken)
    {
        await Task.Yield();

        while (sourceChannel is { CanRead: true } && destinationChannel is { CanWrite: true })
        {
            if (cancellationToken.IsCancellationRequested || forceCancellationToken.IsCancellationRequested)
                break;

            INetworkMessage message;

            var sourceSide = direction is Direction.Serverbound ? Side.Client : Side.Server;
            var destinationSide = direction is Direction.Serverbound ? Side.Server : Side.Client;

            try
            {
                message = await sourceChannel.ReadMessageAsync(forceCancellationToken);

                var cancelled = await events.ThrowWithResultAsync(new MessageReceivedEvent(sourceSide, sourceSide, Side.Proxy, direction, message, this), cancellationToken);

                if (cancelled)
                    continue;
            }
            catch (Exception exception) when (exception is EndOfStreamException or IOException)
            {
                if (direction is Direction.Serverbound)
                {
                    // PlayerChannel.Close();
                    // await PlayerChannel.DisposeAsync();
                }

                // ServerChannel.Close();
                // await ServerChannel.DisposeAsync();
                break;
            }
            catch (Exception exception) when (exception is TaskCanceledException or OperationCanceledException or ObjectDisposedException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled read {Direction} exception in {Link} link", direction, this);
                break;
            }

            // using var sync = await _lock.LockAsync();

            try
            {
                await destinationChannel.WriteMessageAsync(message, forceCancellationToken);

                await events.ThrowAsync(new MessageSentEvent(sourceSide, Side.Proxy, destinationSide, direction, message, this), cancellationToken);
            }
            catch (Exception exception) when (exception is EndOfStreamException or IOException)
            {
                if (direction is Direction.Clientbound)
                {
                    // PlayerChannel.Close();
                    // await PlayerChannel.DisposeAsync();
                }

                // ServerChannel.Close();
                // await ServerChannel.DisposeAsync();
                break;
            }
            catch (Exception exception) when (exception is TaskCanceledException or OperationCanceledException or ObjectDisposedException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled write {Direction} exception in {Link} link", direction, this);
            }
            finally
            {
                message.Dispose();
            }
        }

        // cancellationToken here most likely canceled already
        // use forceCancellationToken for events

        if (!_stopping)
        {
            _stopping = true;
            await events.ThrowAsync(new LinkStoppingEvent(this), forceCancellationToken);
        }

        if (destinationChannel.CanWrite)
            await destinationChannel.FlushAsync(forceCancellationToken);

        switch (direction)
        {
            case Direction.Clientbound:
                _serverToPlayerTaskDisposed = true;
                await DisposeServerboundAsync();
                break;
            case Direction.Serverbound:
                _playerToServerTaskDisposed = true;
                await DisposeClientboundAsync();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        lock (this)
        {
            if (_stopped)
            {
                // do not wait completion as this may start initiating new ILink instance
                var @event = events.ThrowAsync(new LinkStoppedEvent(this), forceCancellationToken).CatchExceptions(logger, $"{nameof(LinkStoppedEvent)} caused exception(s)");
            }

            if (!_stopped)
                _stopped = true;
        }
    }

    public override string ToString()
    {
        return Player + " <=> " + Server;
    }

    private static async ValueTask<bool> WaitWithTimeout(Task task, int milliseconds = 5000)
    {
        var timeout = Task.Delay(milliseconds);
        return await Task.WhenAny(timeout, task) == timeout;
    }
}
