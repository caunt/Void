using MinecraftProxy.Network;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol;
using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.States;
using MinecraftProxy.Network.Protocol.States.Common;
using Nito.AsyncEx;
using System.Net;
using System.Net.Sockets;

namespace MinecraftProxy.Models.General;

public class Link : IDisposable
{
    public Player Player { get; protected set; }
    public Server Server { get; protected set; }
    public MinecraftChannel PlayerChannel { get; protected set; }
    public MinecraftChannel ServerChannel { get; protected set; }
    public ProtocolVersion ProtocolVersion { get; protected set; }
    public ProtocolState State { get; protected set; }
    public ServerInfo? ServerInfo { get; protected set; }
    public EndPoint? PlayerRemoteEndPoint => _client.Client.RemoteEndPoint;
    public EndPoint? ServerRemoteEndPoint => _server.Client.RemoteEndPoint;
    public bool IsSwitching => _redirectionServer != null;

    private AsyncLock _lock = new();

    private TcpClient _client;
    private TcpClient _server;

    private CancellationTokenSource _ctsClientForwarding;
    private CancellationTokenSource _ctsServerForwarding;
    private CancellationTokenSource _ctsClientForwardingForce;
    private CancellationTokenSource _ctsServerForwardingForce;

    private Task _clientForwardingTask;
    private Task _serverForwardingTask;

    private Server? _redirectionServer;
    private HandshakePacket _redirectionHandshakePacket;
    private LoginStartPacket _redirectionLoginStartPacket;

    public Link(TcpClient client, TcpClient server)
    {
        _client = client;
        _server = server;

        PlayerChannel = new MinecraftChannel(_client.GetStream());
        ServerChannel = new MinecraftChannel(_server.GetStream());

        Player = new(this);
        Server = new(this);

        State = SwitchState(0);
        ProtocolVersion = SetProtocolVersion(ProtocolVersion.Oldest);

        _ctsClientForwarding = new();
        _ctsServerForwarding = new();
        _ctsClientForwardingForce = new();
        _ctsServerForwardingForce = new();

        _clientForwardingTask = ForwardClientToServer();
        _serverForwardingTask = ForwardServerToClient();
    }

    public ProtocolVersion SetProtocolVersion(ProtocolVersion protocolVersion)
    {
        return ProtocolVersion = protocolVersion;
    }

    public void SetServerInfo(ServerInfo serverInfo)
    {
        ServerInfo = serverInfo;
    }

    public void SaveHandshake(HandshakePacket packet)
    {
        _redirectionHandshakePacket = packet;
    }

    public void SaveLoginStart(LoginStartPacket packet)
    {
        _redirectionLoginStartPacket = packet;
    }

    public void SwitchComplete()
    {
        _redirectionServer = null;
        _clientForwardingTask = ForwardClientToServer();
    }

    public ProtocolState SwitchState(int state)
    {
        State = state switch
        {
            0 => new HandshakeState(this),
            1 => throw new NotImplementedException("Ping state not implemented yet"),
            2 => new LoginState(this),
            3 => new ConfigurationState(this),
            4 => new PlayState(this),
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };

        Proxy.Logger.Information($"Link {this} switch state to {State.GetType().Name}");

        return State;
    }

    public async Task SwitchServerAsync(ServerInfo serverInfo)
    {
        Proxy.Logger.Information($"Link {this} started server switch for player {Player}");

        var tcpClient = serverInfo.CreateTcpClient();
        var server = new Server(this);

        _redirectionServer = server;

        _ctsClientForwarding.Cancel();
        _ctsServerForwarding.Cancel();

        await _clientForwardingTask;
        await _serverForwardingTask;

        await PlayerChannel.FlushAsync();

        _server.Close();
        _server.Dispose();
        _server = tcpClient;

        Server = server;
        ServerInfo = serverInfo;
        ServerChannel = new(_server.GetStream());

        SwitchState(0);
        await _redirectionServer.SendPacketAsync(_redirectionHandshakePacket);

        SwitchState(2);
        await _redirectionServer.SendPacketAsync(_redirectionLoginStartPacket);

        _serverForwardingTask = ForwardServerToClient();
    }

    public async Task SendPacketAsync(Direction direction, IMinecraftPacket packet)
    {
        var id = State.FindPacketId(direction, packet, ProtocolVersion);

        if (!id.HasValue)
            throw new Exception($"{packet.GetType().Name} packet id not found in {State.GetType().Name}");

        Proxy.Logger.Debug($"Sending {Player} {direction} {packet.GetType().Name}");

        var channel = direction switch
        {
            Direction.Clientbound => PlayerChannel,
            Direction.Serverbound => ServerChannel,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        using var _ = await _lock.LockAsync();
        using var message = MinecraftMessage.Encode(id.Value, packet, direction, ProtocolVersion);
        await channel.WriteMessageAsync(message);
    }

    protected Task ForwardClientToServer() => ProcessPacketsAsync<ProtocolState>(PlayerChannel, ServerChannel, Direction.Serverbound, (_ctsClientForwardingForce = new()).Token, (_ctsClientForwarding = new()).Token);
    protected Task ForwardServerToClient() => ProcessPacketsAsync<ProtocolState>(ServerChannel, PlayerChannel, Direction.Clientbound, (_ctsServerForwardingForce = new()).Token, (_ctsServerForwarding = new()).Token);

    protected async Task ProcessPacketsAsync<T>(MinecraftChannel sourceChannel, MinecraftChannel destinationChannel, Direction direction, CancellationToken forceCancellationToken, CancellationToken cancellationToken) where T : ProtocolState
    {
        Proxy.Logger.Information($"Started forwarding {direction} {Player} traffic");

        try
        {
            while (!cancellationToken.IsCancellationRequested && !forceCancellationToken.IsCancellationRequested && sourceChannel.CanRead && sourceChannel.CanWrite && destinationChannel.CanRead && destinationChannel.CanWrite)
            {
                int length;
                int packetId;
                IMinecraftPacket? packet;

                using (var message = await sourceChannel.ReadMessageAsync(forceCancellationToken))
                {
                    length = message.Length;
                    (packetId, packet, var handleTask) = message.DecodeAndHandle(State, direction, ProtocolVersion);

                    if (direction is Direction.Clientbound && packetId == 0x39) // temp cancel recipe book
                        continue;

                    if (packet is null)
                    {
                        using var _ = await _lock.LockAsync();
                        await destinationChannel.WriteMessageAsync(message, forceCancellationToken);

                        continue;
                    }

                    if (await handleTask)
                    {
                        Proxy.Logger.Debug($"Cancelled {direction} 0x{packetId:X2} packet");
                        continue;
                    }
                }

                using (var message = MinecraftMessage.Encode(packetId, packet, direction, ProtocolVersion))
                {
                    using var _ = await _lock.LockAsync();
                    await destinationChannel.WriteMessageAsync(message, forceCancellationToken);
                }

                if (packet is DisconnectPacket disconnect)
                {
                    await destinationChannel.FlushAsync(cancellationToken);
                    Proxy.Logger.Information($"Player {Player} disconnected from server: {disconnect.Reason}");

                    // shutdown connection with server in case if client forwarding task stops first
                    _ctsServerForwardingForce.Cancel();

                    return;
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

            if (!IsSwitching)
            {
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
