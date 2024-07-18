using Nito.AsyncEx;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;
using Void.Proxy.Network;

namespace Void.Proxy.Links;

public class Link(
    IPlayer player,
    IServer server,
    IMinecraftChannel playerChannel,
    IMinecraftChannel serverChannel,
    Func<ILink, ValueTask> finalize) : ILink
{
    private readonly AsyncLock _lock = new();
    private readonly ILogger<Link> _logger = player.Scope.ServiceProvider.GetRequiredService<ILogger<Link>>();

    private Task? _clientForwardingTask;

    private CancellationTokenSource? _ctsClientForwarding;
    private CancellationTokenSource? _ctsClientForwardingForce;
    private CancellationTokenSource? _ctsServerForwarding;
    private CancellationTokenSource? _ctsServerForwardingForce;
    private Task? _serverForwardingTask;
    public IPlayer Player => player;
    public IServer Server => server;
    public IMinecraftChannel PlayerChannel => playerChannel;
    public IMinecraftChannel ServerChannel => serverChannel;

    public ProtocolVersion ProtocolVersion => player.ProtocolVersion;

    public void StartForwarding()
    {
        _ctsClientForwarding = new CancellationTokenSource();
        _ctsServerForwarding = new CancellationTokenSource();
        _ctsClientForwardingForce = new CancellationTokenSource();
        _ctsServerForwardingForce = new CancellationTokenSource();

        _clientForwardingTask = ForwardClientToServer();
        _serverForwardingTask = ForwardServerToClient();
    }

    public async ValueTask DisposeAsync()
    {
        serverChannel.Close();
        await serverChannel.DisposeAsync();
    }

    protected async Task StopClientToServerForwarding()
    {
        if (_ctsClientForwarding is not null)
            await _ctsClientForwarding.CancelAsync();

        if (_clientForwardingTask is not null)
            await _clientForwardingTask;

        await PlayerChannel.FlushAsync();
    }

    protected async Task StopServerToClientForwarding()
    {
        if (_ctsServerForwarding is not null)
            await _ctsServerForwarding.CancelAsync();

        if (_serverForwardingTask is not null)
            await _serverForwardingTask;

        await ServerChannel.FlushAsync();
    }

    protected Task ForwardClientToServer()
    {
        _ctsClientForwardingForce = new CancellationTokenSource();
        _ctsClientForwarding = new CancellationTokenSource();

        return ExecuteAsync(PlayerChannel, ServerChannel, Direction.Serverbound, _ctsClientForwardingForce.Token, _ctsClientForwarding.Token);
    }

    protected Task ForwardServerToClient()
    {
        _ctsServerForwardingForce = new CancellationTokenSource();
        _ctsServerForwarding = new CancellationTokenSource();

        return ExecuteAsync(ServerChannel, PlayerChannel, Direction.Clientbound, _ctsServerForwardingForce.Token, _ctsServerForwarding.Token);
    }

    protected async Task ExecuteAsync(IMinecraftChannel sourceChannel, IMinecraftChannel destinationChannel, Direction direction, CancellationToken forceCancellationToken, CancellationToken cancellationToken)
    {
        await Task.Yield();
        _logger.LogInformation("Started forwarding {Direction} {Player} traffic", direction, player);

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

                using var _ = await _lock.LockAsync();
                await destinationChannel.WriteMessageAsync(message /*, forceCancellationToken*/);
            }
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or TaskCanceledException or OperationCanceledException)
        {
            // client disconnected itself
            // server catch unhandled exception
            // link does server switch
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled {Direction} exception from {Player}", direction, player);
        }
        finally
        {
            _logger.LogInformation("Stopped forwarding {Direction} {Player} traffic", direction, player);

            // TODO do not wait client, rewrite this to close just Clientbound (Server) connection
            // one side disconnected, shutting down link, if its not server redirection case

            var thisCancellationTokenSource = direction is Direction.Serverbound ? _ctsClientForwardingForce : _ctsServerForwardingForce;
            var otherCancellationTokenSource = direction is Direction.Serverbound ? _ctsServerForwardingForce : _ctsClientForwardingForce;
            var thisForwardingTask = direction is Direction.Serverbound ? _clientForwardingTask : _serverForwardingTask;
            var otherForwardingTask = direction is Direction.Serverbound ? _serverForwardingTask : _clientForwardingTask;

            await thisCancellationTokenSource!.CancelAsync();

            if (!otherCancellationTokenSource!.IsCancellationRequested)
            {
                var timeout = Task.Delay(5000);
                var disconnectTask = await Task.WhenAny(otherForwardingTask!, timeout);

                if (disconnectTask == timeout)
                {
                    _logger.LogInformation("Timed out waiting {Side} {Player} disconnection, closing manually", direction is Direction.Serverbound ? "Server" : "Player", player);
                    await otherCancellationTokenSource.CancelAsync();
                }
            }

            await finalize(this);
        }
    }
}