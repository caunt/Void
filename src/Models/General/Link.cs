using MinecraftProxy.Network;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol;
using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.States;
using MinecraftProxy.Network.Protocol.States.Common;
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

    private TcpClient _client;
    private TcpClient _server;

    private CancellationTokenSource _clientCancellationTokenSource;
    private CancellationTokenSource _serverCancellationTokenSource;

    private Task _clientForwardingTask;
    private Task _serverForwardingTask;

    private Server? _redirectionServer;

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

        _clientCancellationTokenSource = new();
        _serverCancellationTokenSource = new();

        _clientForwardingTask = ProcessPacketsAsync<ProtocolState>(PlayerChannel, ServerChannel, Direction.Serverbound, _clientCancellationTokenSource.Token);
        _serverForwardingTask = ProcessPacketsAsync<ProtocolState>(ServerChannel, PlayerChannel, Direction.Clientbound, _serverCancellationTokenSource.Token);
    }

    public ProtocolVersion SetProtocolVersion(ProtocolVersion protocolVersion)
    {
        return ProtocolVersion = protocolVersion;
    }

    public void SetServerInfo(ServerInfo serverInfo)
    {
        ServerInfo = serverInfo;
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
        Proxy.Logger.Information($"Link {this} executed server switch for player {Player}");

        var tcpClient = serverInfo.CreateTcpClient();
        var server = new Server(this);

        Proxy.Logger.Debug($"Link {this} stopping previous server forwarding for player {Player}");
        _redirectionServer = server;
        _serverCancellationTokenSource.Cancel();

        Proxy.Logger.Debug($"Link {this} awaiting forwarding completion for player {Player}");
        await _serverForwardingTask;

        SwitchState(0);
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

        using var message = MinecraftMessage.Encode(id.Value, packet, Direction.Clientbound, ProtocolVersion);
        await channel.WriteMessageAsync(message);
    }

    protected async Task ProcessPacketsAsync<T>(MinecraftChannel sourceChannel, MinecraftChannel destinationChannel, Direction direction, CancellationToken cancellationToken) where T : ProtocolState
    {
        Proxy.Logger.Information($"Started forwarding {direction} {Player} traffic");

        try
        {
            while (!cancellationToken.IsCancellationRequested && sourceChannel.CanRead && sourceChannel.CanWrite && destinationChannel.CanRead && destinationChannel.CanWrite)
            {
                int length;
                int packetId;
                IMinecraftPacket? packet;

                using (var message = await sourceChannel.ReadMessageAsync(cancellationToken))
                {
                    length = message.Length;
                    (packetId, packet, var handleTask) = message.DecodeAndHandle(State, direction, ProtocolVersion);

                    if (packet is null)
                    {
                        await destinationChannel.WriteMessageAsync(message, cancellationToken);
                        continue;
                    }

                    if (await handleTask)
                        continue;
                }

                using (var message = MinecraftMessage.Encode(packetId, packet, direction, ProtocolVersion, length + 2048))
                {
                    await destinationChannel.WriteMessageAsync(message, cancellationToken);
                }

                if (packet is DisconnectPacket disconnect)
                {
                    await destinationChannel.FlushAsync(cancellationToken);
                    Proxy.Logger.Information($"Player {Player} disconnected from server: {disconnect.Reason}");

                    // shutdown connection with server
                    _serverCancellationTokenSource.Cancel();

                    return;
                }
            }
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or TaskCanceledException)
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
            if (direction is Direction.Serverbound || _redirectionServer == null)
            {
                // one side disconnected, shutting down link, if its not server redirection case

                var thisCancellationTokenSource = direction is Direction.Serverbound ? _clientCancellationTokenSource : _serverCancellationTokenSource;
                var otherCancellationTokenSource = direction is Direction.Serverbound ? _serverCancellationTokenSource : _clientCancellationTokenSource;
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

            Proxy.Logger.Information($"Stopped forwarding {direction} {Player} traffic");
        }
    }

    public void Dispose()
    {
        _client.Close();
        _server.Close();
        _client.Dispose();
        _server.Dispose();

        GC.SuppressFinalize(this);
    }
}
