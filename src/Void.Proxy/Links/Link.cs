using Nito.AsyncEx;
using System.Threading.Tasks;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;
using Void.Proxy.Network;

namespace Void.Proxy.Links;

public class Link : ILink
{
    public IPlayer Player { get; init; }
    public IServer Server { get; init; }
    public IMinecraftChannel PlayerChannel { get; init; }
    public IMinecraftChannel ServerChannel { get; init; }

    private readonly Func<ILink, ValueTask> _finalizer;
    private readonly ILogger<Link> _logger;
    private readonly AsyncLock _lock;

    private readonly Task _playerToServerTask;
    private readonly Task _serverToPlayerTask;

    private readonly CancellationTokenSource _cts;
    private readonly CancellationTokenSource _ctsForce;

    public Link(IPlayer player, IServer server, IMinecraftChannel playerChannel, IMinecraftChannel serverChannel, Func<ILink, ValueTask> finalize)
    {
        Player = player;
        Server = server;
        PlayerChannel = playerChannel;
        ServerChannel = serverChannel;

        _lock = new AsyncLock();
        _finalizer = finalize;
        _logger = player.Scope.ServiceProvider.GetRequiredService<ILogger<Link>>();

        _cts = new CancellationTokenSource();
        _ctsForce = new CancellationTokenSource();
        
        _playerToServerTask = ExecuteAsync(PlayerChannel, ServerChannel, Direction.Serverbound, _cts.Token, _ctsForce.Token);
        _serverToPlayerTask = ExecuteAsync(ServerChannel, PlayerChannel, Direction.Clientbound, _cts.Token, _ctsForce.Token);
    }
    
    public override string ToString() => Player + " <=> " + Server;

    public async ValueTask DisposeAsync()
    {
        ServerChannel.Close();
        await ServerChannel.DisposeAsync();

        var timeout = Task.Delay(5000);

        if (await Task.WhenAny(timeout, _serverToPlayerTask) == timeout)
        {
            _logger.LogInformation("Timed out waiting Server {Server} disconnection from Player {Player}, closing manually", Server, Player);
            await _ctsForce.CancelAsync();
        }

        if (await Task.WhenAny(timeout, _playerToServerTask) == timeout)
        {
            _logger.LogInformation("Timed out waiting Player {Player} disconnection from Server {Server}, closing manually", Player, Server);
            await _ctsForce.CancelAsync();
        }
        
        await Task.WhenAll(_playerToServerTask, _serverToPlayerTask);
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

                // TODO add dispose
                var message = await sourceChannel.ReadMessageAsync();

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
            // server catch unhandled exception
            // link does server switch
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled {Direction} exception from {Player}", direction, Player);
        }
        finally
        {
            // TODO sometimes kick packet do not reach the player
            
            await PlayerChannel.FlushAsync();
            await ServerChannel.FlushAsync();

            await _cts.CancelAsync();
            _ = _finalizer(this);
        }
    }
}