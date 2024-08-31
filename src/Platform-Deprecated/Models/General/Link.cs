using System.Net;
using System.Net.Sockets;
using Nito.AsyncEx;
using Void.Proxy.Network;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol;
using Void.Proxy.Network.Protocol.Packets;
using Void.Proxy.Network.Protocol.Packets.Clientbound;
using Void.Proxy.Network.Protocol.Packets.Serverbound;
using Void.Proxy.Network.Protocol.States;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Models.General;

public class Link : IDisposable
{
    private readonly TcpClient _client;
    private readonly AsyncLock _lock = new();

    private Task _clientForwardingTask;

    private CancellationTokenSource _ctsClientForwarding;
    private CancellationTokenSource _ctsClientForwardingForce;
    private CancellationTokenSource _ctsServerForwarding;
    private CancellationTokenSource _ctsServerForwardingForce;

    private HandshakePacket _redirectionHandshakePacket;
    private LoginStartPacket _redirectionLoginStartPacket;

    private Server? _redirectionServer;
    private ServerInfo? _redirectionServerInfo;
    private TcpClient _server;
    private Task _serverForwardingTask;

    public Link(TcpClient client, TcpClient server)
    {
        _client = client;
        _server = server;

        PlayerChannel = new MinecraftChannel(_client.GetStream());
        ServerChannel = new MinecraftChannel(_server.GetStream());

        Player = new Player(this);
        Server = new Server(this);

        State = SwitchState(0);
        ProtocolVersion = SetProtocolVersion(ProtocolVersion.Oldest);

        _ctsClientForwarding = new CancellationTokenSource();
        _ctsServerForwarding = new CancellationTokenSource();
        _ctsClientForwardingForce = new CancellationTokenSource();
        _ctsServerForwardingForce = new CancellationTokenSource();

        _clientForwardingTask = ForwardClientToServer();
        _serverForwardingTask = ForwardServerToClient();
    }

    public Player Player { get; protected set; }
    public Server Server { get; protected set; }
    public MinecraftChannel PlayerChannel { get; protected set; }
    public MinecraftChannel ServerChannel { get; protected set; }
    public ProtocolVersion ProtocolVersion { get; protected set; }
    public ProtocolState State { get; protected set; }
    public ServerInfo? ServerInfo { get; protected set; }
    public EndPoint? PlayerRemoteEndPoint => _client.Client?.RemoteEndPoint;
    public EndPoint? ServerRemoteEndPoint => _server.Client?.RemoteEndPoint;
    public bool IsSwitching => _redirectionServer != null;

    public void Dispose()
    {
        Proxy.Logger.Debug($"Link {GetHashCode()} with player {Player} disposing");

        _client.Close();
        _server.Close();
        _client.Dispose();
        _server.Dispose();

        GC.SuppressFinalize(this);
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

        _redirectionServer = new Server(this);
        _redirectionServerInfo = serverInfo;

        await StopServerToClientForwarding();

        if (ProtocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
        {
            await Player.SendPacketAsync(new StartConfiguration());
        }
        else
        {
            await StopClientToServerForwarding();
            await StartServerSwitchAsync();
        }
    }

    public async Task ReplaceRedirectionServerChannel()
    {
        await StopClientToServerForwarding();
        await StartServerSwitchAsync();
    }

    public async Task ReplaceRedirectionClientChannel()
    {
        // redirection server waiting us here for login acknowledge
        await Server.SendPacketAsync(new LoginAcknowledgedPacket());

        SwitchState(3);
        SwitchComplete();
    }

    public async Task StartServerSwitchAsync()
    {
        if (_redirectionServer is null)
            throw new Exception("Not found redirection server to complete");

        if (_redirectionServerInfo is null)
            throw new Exception("Not found redirection server info to complete");

        var tcpClient = _redirectionServerInfo.CreateTcpClient();

        _server.Close();
        _server.Dispose();
        _server = tcpClient;

        Server = _redirectionServer;
        ServerInfo = _redirectionServerInfo;
        ServerChannel = new MinecraftChannel(_server.GetStream());

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
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

        using var _ = await _lock.LockAsync();
        using var message = MinecraftMessage.Encode(id.Value, packet, direction, ProtocolVersion);
        await channel.WriteMessageAsync(message);
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

    protected Task ForwardClientToServer()
    {
        return ProcessPacketsAsync<ProtocolState>(PlayerChannel, ServerChannel, Direction.Serverbound, (_ctsClientForwardingForce = new CancellationTokenSource()).Token, (_ctsClientForwarding = new CancellationTokenSource()).Token);
    }

    protected Task ForwardServerToClient()
    {
        return ProcessPacketsAsync<ProtocolState>(ServerChannel, PlayerChannel, Direction.Clientbound, (_ctsServerForwardingForce = new CancellationTokenSource()).Token, (_ctsServerForwarding = new CancellationTokenSource()).Token);
    }

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
}