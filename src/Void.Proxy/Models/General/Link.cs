using Nito.AsyncEx;
using System.Net;
using System.Net.Sockets;
using Void.Proxy.Network;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol;

namespace Void.Proxy.Models.General;

public class Link : IDisposable
{
    public Player Player { get; protected set; }
    public Server? Server { get; protected set; }
    public MinecraftChannel PlayerChannel { get; protected set; }
    public MinecraftChannel? ServerChannel { get; protected set; }
    public ProtocolVersion? ProtocolVersion { get; protected set; }
    public ServerInfo? ServerInfo { get; protected set; }
    public EndPoint? PlayerRemoteEndPoint => _client.Client?.RemoteEndPoint;
    public EndPoint? ServerRemoteEndPoint => _server?.Client?.RemoteEndPoint;

    private AsyncLock _lock = new();

    private TcpClient _client;
    private TcpClient? _server;

    private CancellationTokenSource? _ctsClientForwarding;
    private CancellationTokenSource? _ctsServerForwarding;
    private CancellationTokenSource? _ctsClientForwardingForce;
    private CancellationTokenSource? _ctsServerForwardingForce;

    private Task? _clientForwardingTask;
    private Task? _serverForwardingTask;

    public Link(TcpClient client)
    {
        _client = client;
        PlayerChannel = new MinecraftChannel(_client.GetStream());
        Player = new(this);
    }

    public void Connect(ServerInfo serverInfo)
    {
        ServerInfo = serverInfo;

        _server = serverInfo.CreateTcpClient();
        ServerChannel = new MinecraftChannel(_server.GetStream());
        Server = new(this);
    }

    public void StartForwarding()
    {
        _ctsClientForwarding = new();
        _ctsServerForwarding = new();
        _ctsClientForwardingForce = new();
        _ctsServerForwardingForce = new();

        _clientForwardingTask = ForwardClientToServer();
        _serverForwardingTask = ForwardServerToClient();
    }

    protected async Task StopClientToServerForwarding()
    {
        _ctsClientForwarding.Cancel();
        await _clientForwardingTask;
        await PlayerChannel.FlushAsync();
    }

    protected async Task StopServerToClientForwarding()
    {
        _ctsServerForwarding.Cancel();
        await _serverForwardingTask;
        await ServerChannel.FlushAsync();
    }

    protected Task ForwardClientToServer() => ProcessPacketsAsync(PlayerChannel, ServerChannel, Direction.Serverbound, (_ctsClientForwardingForce = new()).Token, (_ctsClientForwarding = new()).Token);
    protected Task ForwardServerToClient() => ProcessPacketsAsync(ServerChannel, PlayerChannel, Direction.Clientbound, (_ctsServerForwardingForce = new()).Token, (_ctsServerForwarding = new()).Token);

    protected async Task ProcessPacketsAsync(MinecraftChannel sourceChannel, MinecraftChannel destinationChannel, Direction direction, CancellationToken forceCancellationToken, CancellationToken cancellationToken)
    {
        Proxy.Logger.Information($"Started forwarding {direction} {Player} traffic");

        try
        {
            while (!cancellationToken.IsCancellationRequested && !forceCancellationToken.IsCancellationRequested && sourceChannel.CanRead && sourceChannel.CanWrite && destinationChannel.CanRead && destinationChannel.CanWrite)
            {
                using (var message = await sourceChannel.ReadMessageAsync(forceCancellationToken))
                {
                    using var _ = await _lock.LockAsync();
                    await destinationChannel.WriteMessageAsync(message, forceCancellationToken);
                }
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
            Proxy.Logger.Information($"Unhandled exception from {direction} channel ({Player}):\n{exception}");
        }
        finally
        {
            Proxy.Logger.Information($"Stopped forwarding {direction} {Player} traffic");

            // one side disconnected, shutting down link, if its not server redirection case

            var thisCancellationTokenSource = direction is Direction.Serverbound ? _ctsClientForwardingForce : _ctsServerForwardingForce;
            var otherCancellationTokenSource = direction is Direction.Serverbound ? _ctsServerForwardingForce : _ctsClientForwardingForce;
            var thisForwardingTask = direction is Direction.Serverbound ? _clientForwardingTask : _serverForwardingTask;
            var otherForwardingTask = direction is Direction.Serverbound ? _serverForwardingTask : _clientForwardingTask;

            thisCancellationTokenSource.Cancel();

            if (!otherCancellationTokenSource.IsCancellationRequested)
            {
                var timeout = Task.Delay(5000);
                var disconnectTask = await Task.WhenAny(otherForwardingTask, timeout);

                if (disconnectTask == timeout)
                {
                    Proxy.Logger.Information($"Timed out waiting {(direction is Direction.Serverbound ? "Server" : "Player")} {Player} disconnection, closing manually");
                    otherCancellationTokenSource.Cancel();
                }
            }

            Proxy.Links.Remove(this);
            Dispose();
        }
    }

    public void Dispose()
    {
        Proxy.Logger.Debug($"Link {this.GetHashCode()} with player {Player} disposing");

        _client.Close();
        _server.Close();
        _client.Dispose();
        _server.Dispose();

        GC.SuppressFinalize(this);
    }
}
