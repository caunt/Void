using Nito.AsyncEx;
using Void.Proxy.API.Events.Links;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;
using Void.Proxy.Network;

namespace Void.Proxy.Links;

public class Link : ILink
{
    private readonly AsyncLock _lock;
    private readonly CancellationTokenSource _ctsPlayerToServer;
    private readonly CancellationTokenSource _ctsPlayerToServerForce;
    private readonly CancellationTokenSource _ctsServerToPlayer;
    private readonly CancellationTokenSource _ctsServerToPlayerForce;
    private readonly IEventService _events;

    private readonly ILogger<Link> _logger;

    private readonly Task _playerToServerTask;
    private readonly Task _serverToPlayerTask;

    public Link(IPlayer player, IServer server, IMinecraftChannel playerChannel, IMinecraftChannel serverChannel, IEventService events)
    {
        Player = player;
        Server = server;
        PlayerChannel = playerChannel;
        ServerChannel = serverChannel;

        _logger = player.Scope.ServiceProvider.GetRequiredService<ILogger<Link>>();
        _events = events;
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

    public async ValueTask DisposeAsync()
    {
        ServerChannel.Close();
        await ServerChannel.DisposeAsync();

        if (await WaitWithTimeout(_serverToPlayerTask))
        {
            _logger.LogInformation("Timed out waiting Server {Server} disconnection from Player {Player}, closing manually", Server, Player);
            await _ctsPlayerToServer.CancelAsync();

            if (await WaitWithTimeout(_serverToPlayerTask))
            {
                _logger.LogInformation("Timed out waiting Server {Server} disconnection from Player {Player} manually, closing forcefully", Server, Player);
                await _ctsPlayerToServerForce.CancelAsync();

                if (await WaitWithTimeout(_serverToPlayerTask))
                    throw new Exception($"Cannot dispose Link {this} (player=>server)");
            }
        }

        if (await WaitWithTimeout(_playerToServerTask))
        {
            _logger.LogInformation("Timed out waiting Player {Player} disconnection from Server {Server}, closing manually", Player, Server);
            await _ctsServerToPlayer.CancelAsync();

            if (await WaitWithTimeout(_playerToServerTask))
            {
                _logger.LogInformation("Timed out waiting Player {Player} disconnection from Server {Server} manually, closing forcefully", Player, Server);
                await _ctsServerToPlayerForce.CancelAsync();

                if (await WaitWithTimeout(_playerToServerTask))
                    throw new Exception($"Cannot dispose Link {this} (server=>player)");
            }
        }

        await Task.WhenAll(_playerToServerTask, _serverToPlayerTask);
        return;

        static async Task<bool> WaitWithTimeout(Task task, int milliseconds = 5000)
        {
            var timeout = Task.Delay(milliseconds);
            return await Task.WhenAny(timeout, task) == timeout;
        }
    }

    public override string ToString()
    {
        return Player + " <=> " + Server;
    }

    protected async Task ExecuteAsync(IMinecraftChannel sourceChannel, IMinecraftChannel destinationChannel, Direction direction, CancellationToken cancellationToken, CancellationToken forceCancellationToken)
    {
        await Task.Yield();

        try
        {
            while (sourceChannel is { CanRead: true, CanWrite: true } && destinationChannel is { CanRead: true, CanWrite: true })
            {
                if (cancellationToken.IsCancellationRequested || forceCancellationToken.IsCancellationRequested)
                    break;

                using var message = await sourceChannel.ReadMessageAsync();

                if (message is BinaryPacket packet)
                    _logger.LogDebug("{Direction} packet id {PacketId}", direction, packet.Id);
                else if (message is BufferedBinaryMessage binary)
                    _logger.LogDebug("{Direction} packet length {Length}", direction, binary.Memory.Length);

                using var _ = await _lock.LockAsync();
                await destinationChannel.WriteMessageAsync(message /*, forceCancellationToken*/);
            }
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or TaskCanceledException or OperationCanceledException or ObjectDisposedException)
        {
            // client disconnected itself
            // or server catch unhandled exception
            // or link does server switch
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled {Direction} exception from {Player}", direction, Player);
        }
        finally
        {
            await PlayerChannel.FlushAsync();
            await ServerChannel.FlushAsync();

            _ = _events.ThrowAsync(new LinkStoppingEvent { Link = this }, forceCancellationToken);
        }
    }
}