using Nito.AsyncEx;
using Void.Proxy.API.Events.Links;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.Links;

public class Link(IPlayer player, IServer server, IMinecraftChannel playerChannel, IMinecraftChannel serverChannel, ILogger logger, IEventService events) : ILink
{
    private readonly CancellationTokenSource _ctsPlayerToServer = new();
    private readonly CancellationTokenSource _ctsPlayerToServerForce = new();
    private readonly CancellationTokenSource _ctsServerToPlayer = new();
    private readonly CancellationTokenSource _ctsServerToPlayerForce = new();
    private readonly AsyncLock _lock = new();

    private Task? _playerToServerTask;
    private Task? _serverToPlayerTask;

    public IPlayer Player { get; init; } = player;
    public IServer Server { get; init; } = server;
    public IMinecraftChannel PlayerChannel { get; init; } = playerChannel;
    public IMinecraftChannel ServerChannel { get; init; } = serverChannel;

    public bool IsAlive => _playerToServerTask?.Status == TaskStatus.Running && _serverToPlayerTask?.Status == TaskStatus.Running;

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        if (this is { _playerToServerTask: not null } or { _serverToPlayerTask: not null })
            throw new InvalidOperationException("Link was already started");

        await events.ThrowAsync(new LinkStartingEvent { Link = this }, cancellationToken);

        events.RegisterListeners(this);

        _playerToServerTask = ExecuteAsync(PlayerChannel, ServerChannel, Direction.Serverbound, _ctsPlayerToServer.Token, _ctsPlayerToServerForce.Token);
        _serverToPlayerTask = ExecuteAsync(ServerChannel, PlayerChannel, Direction.Clientbound, _ctsServerToPlayer.Token, _ctsServerToPlayerForce.Token);

        logger.LogInformation("Started forwarding {Link} traffic", this);
        await events.ThrowAsync(new LinkStartedEvent { Link = this }, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        // if the player does not support redirections, that's the end for him
        if (true /*!PlayerChannel.IsRedirectionSupported*/)
        {
            PlayerChannel.Close();
            await PlayerChannel.DisposeAsync();
        }

        ServerChannel.Close();
        await ServerChannel.DisposeAsync();

        if (_serverToPlayerTask is not null)
        {
            if (await WaitWithTimeout(_serverToPlayerTask))
            {
                logger.LogTrace("Timed out waiting Server {Server} disconnection from Player {Player}, closing manually", Server, Player);
                await _ctsServerToPlayer.CancelAsync();

                if (await WaitWithTimeout(_serverToPlayerTask))
                {
                    logger.LogTrace("Timed out waiting Server {Server} disconnection from Player {Player} manually, closing forcefully", Server, Player);
                    await _ctsServerToPlayerForce.CancelAsync();

                    if (await WaitWithTimeout(_serverToPlayerTask))
                        throw new Exception($"Cannot dispose Link {this} (player=>server)");
                }
            }

            await _serverToPlayerTask;
        }

        if (_playerToServerTask is not null)
        {
            if (await WaitWithTimeout(_playerToServerTask))
            {
                logger.LogTrace("Timed out waiting Player {Player} disconnection from Server {Server}, closing manually", Player, Server);
                await _ctsPlayerToServer.CancelAsync();

                if (await WaitWithTimeout(_playerToServerTask))
                {
                    logger.LogTrace("Timed out waiting Player {Player} disconnection from Server {Server} manually, closing forcefully", Player, Server);
                    await _ctsPlayerToServerForce.CancelAsync();

                    if (await WaitWithTimeout(_playerToServerTask))
                        throw new Exception($"Cannot dispose Link {this} (server=>player)");
                }
            }

            await _playerToServerTask;
        }
    }

    protected async Task ExecuteAsync(IMinecraftChannel sourceChannel, IMinecraftChannel destinationChannel, Direction direction, CancellationToken cancellationToken, CancellationToken forceCancellationToken)
    {
        await Task.Yield();

        // not sure if reading should be cancelled with grace token, but helps with link restarts after compression enabled
        var readingCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, forceCancellationToken);

        try
        {
            while (sourceChannel is { CanRead: true, CanWrite: true } && destinationChannel is { CanRead: true, CanWrite: true })
            {
                if (cancellationToken.IsCancellationRequested || forceCancellationToken.IsCancellationRequested)
                    break;

                using var message = await sourceChannel.ReadMessageAsync(readingCancellationTokenSource.Token);

                var cancelled = await events.ThrowWithResultAsync(new MessageReceivedEvent
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

                await events.ThrowAsync(new MessageSentEvent
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
            if (direction is not Direction.Serverbound)
                return;

            PlayerChannel.Close();
            await PlayerChannel.DisposeAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled {Direction} exception from {Player}", direction, Player);
        }
        finally
        {
            await PlayerChannel.FlushAsync(forceCancellationToken);
            await ServerChannel.FlushAsync(forceCancellationToken);

            if (sourceChannel == PlayerChannel) // throw event only once
                _ = events.ThrowAsync(new LinkStoppingEvent { Link = this }, forceCancellationToken).CatchExceptions();
        }
    }

    public override string ToString()
    {
        return Player + " <=> " + Server;
    }

    private static async Task<bool> WaitWithTimeout(Task task, int milliseconds = 5000)
    {
        var timeout = Task.Delay(milliseconds);
        return await Task.WhenAny(timeout, task) == timeout;
    }
}