using MinecraftProxy.Models;
using MinecraftProxy.Network;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol;
using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.States;
using MinecraftProxy.Network.Protocol.States.Common;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;

namespace MinecraftProxy;

public class Player : IDisposable
{
    public string? Brand { get; protected set; }
    public GameProfile? GameProfile { get; protected set; }
    public ProtocolState? State { get; protected set; }
    public EndPoint? RemoteEndPoint => tcpClient.Client.RemoteEndPoint;
    public Server? CurrentServer { get; protected set; }
    public IdentifiedKey? IdentifiedKey { get; protected set; }
    public ProtocolVersion ProtocolVersion { get; protected set; }

    protected readonly TcpClient tcpClient;
    protected MinecraftChannel? channel;

    public Player(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;

        State = SwitchState(0);
        ProtocolVersion = SetProtocolVersion(ProtocolVersion.Oldest);
    }

    public void SetGameProfile(GameProfile gameProfile)
    {
        GameProfile = gameProfile;
    }

    public void SetBrand(string brand)
    {
        Brand = brand;
    }

    public ProtocolVersion SetProtocolVersion(ProtocolVersion protocolVersion)
    {
        return ProtocolVersion = protocolVersion;
    }

    public ProtocolState SwitchState(int state)
    {
        State = state switch
        {
            0 => new HandshakeState(this, CurrentServer),
            1 => throw new NotImplementedException("Ping state not implemented yet"),
            2 => new LoginState(this, CurrentServer),
            3 => new ConfigurationState(this, CurrentServer),
            4 => new PlayState(this, CurrentServer),
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };

        Proxy.Logger.Information($"Player {this} switch state to {State.GetType().Name}");

        return State;
    }

    public void EnableEncryption(byte[] secret)
    {
        ArgumentNullException.ThrowIfNull(channel);

        channel.EnableEncryption(secret);
        Proxy.Logger.Information($"Player {this} enabled encryption");
    }

    public void EnableCompression(int threshold)
    {
        ArgumentNullException.ThrowIfNull(channel);

        channel.EnableCompression(threshold);
        Proxy.Logger.Information($"Player {this} enabled compression");
    }

    public void SetIdentifiedKey(IdentifiedKey identifiedKey)
    {
        IdentifiedKey = identifiedKey;
    }

    public async Task<GameProfile?> RequestGameProfileAsync(byte[] secret)
    {
        if (GameProfile is null)
            throw new Exception("Can't request online GameProfile without any GameProfile received before (usually sent by player in login packet)");

        static byte[] TwosComplement(byte[] data)
        {
            int i;
            bool carry = true;

            for (i = data.Length - 1; i >= 0; i--)
            {
                data[i] = unchecked((byte)~data[i]);
                if (carry)
                {
                    carry = data[i] == 0xFF;
                    data[i]++;
                }
            }

            return data;
        }

        var serverId = SHA1.HashData([.. secret, .. Proxy.RSA.ExportSubjectPublicKeyInfo()]);

        var negative = (serverId[0] & 0x80) == 0x80;
        if (negative)
            serverId = TwosComplement(serverId);

        var serverIdComplement = Convert.ToHexString(serverId).TrimStart('0');

        if (negative)
            serverIdComplement = "-" + serverIdComplement;

        var url = $"{Environment.GetEnvironmentVariable("mojang.sessionserver") ?? "https://sessionserver.mojang.com/session/minecraft/hasJoined"}?username={GameProfile.Name}&serverId={serverIdComplement}";

        var playerIp = RemoteEndPoint switch { IPEndPoint ipEndPoint => ipEndPoint.Address.ToString(), _ => null };
        var preventProxyConnections = false;

        if (preventProxyConnections && playerIp is not null)
            url += "&ip=" + playerIp;

        var response = await Proxy.HttpClient.GetAsync(url);

        if (response.StatusCode is HttpStatusCode.NoContent)
            throw new Exception("Offline user connected to online-mode proxy");

        GameProfile = JsonSerializer.Deserialize<GameProfile>(await response.Content.ReadAsStreamAsync(), Proxy.JsonSerializerOptions);

        if (GameProfile != null && IdentifiedKey != null && IdentifiedKey.Revision == IdentifiedKeyRevision.LINKED_V2)
        {
            if (!IdentifiedKey.AddGuid(GameProfile.Id))
                throw new Exception("multiplayer.disconnect.invalid_public_key");
        }

        return GameProfile;
    }

    public async Task SendPacketAsync(IMinecraftPacket packet)
    {
        ArgumentNullException.ThrowIfNull(State);
        ArgumentNullException.ThrowIfNull(ProtocolVersion);

        var id = State.FindPacketId(Direction.Clientbound, packet, ProtocolVersion);

        if (!id.HasValue)
            throw new Exception($"{packet.GetType().Name} packet id not found in {State.GetType().Name}");

        Proxy.Logger.Debug($"Sending {packet.GetType().Name} to player {this}");
        ArgumentNullException.ThrowIfNull(channel);

        using var message = MinecraftMessage.Encode(id.Value, packet, Direction.Clientbound, ProtocolVersion);
        await channel.WriteMessageAsync(message);
    }

    public async Task ForwardTrafficAsync(Server server)
    {
        CurrentServer = server;

        channel = new MinecraftChannel(tcpClient.GetStream());
        var cts = new CancellationTokenSource();

        var clientTask = ProcessPacketsAsync<ProtocolState>(channel, server.GetChannel(), "Player", cts.Token);
        var serverTask = ProcessPacketsAsync<ProtocolState>(server.GetChannel(), channel, "Server", cts.Token);
        var completedTask = await Task.WhenAny(serverTask, clientTask);

        if (completedTask.IsFaulted && completedTask.Exception.InnerExceptions.All(exception => exception is not EndOfStreamException and not IOException))
            Proxy.Logger.Information($"Unhandled exception while reading {(completedTask == serverTask ? "Server" : "Client")} channel ({this}):\n{completedTask.Exception}");

        // graceful opposite disconnection timeout
        var timeout = Task.Delay(5000);
        var disconnectTask = await Task.WhenAny(completedTask == serverTask ? clientTask : serverTask, timeout);

        if (disconnectTask == timeout)
        {
            cts.Cancel();
            Proxy.Logger.Information($"Timed out waiting {(completedTask == serverTask ? "Client" : "Server")} disconnection");
        }

        Proxy.Logger.Information($"Stopped forwarding traffic from/to {this}");
    }

    public override string ToString() => GameProfile?.Name ?? tcpClient.Client?.RemoteEndPoint?.ToString() ?? "Disposed?";

    protected async Task ProcessPacketsAsync<T>(MinecraftChannel sourceChannel, MinecraftChannel destinationChannel, string sourceIdentifier, CancellationToken cancellationToken) where T : ProtocolState
    {
        ArgumentNullException.ThrowIfNull(State);
        ArgumentNullException.ThrowIfNull(ProtocolVersion);

        var direction = sourceIdentifier switch
        {
            "Player" => Direction.Serverbound,
            "Server" => Direction.Clientbound,
            _ => throw new ArgumentException(sourceIdentifier)
        };

        while (sourceChannel.CanRead && sourceChannel.CanWrite && destinationChannel.CanRead && destinationChannel.CanWrite)
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
                Proxy.Logger.Information($"Player {this} disconnected from server: {disconnect.Reason}");
                break;
            }
        }
    }

    public void Dispose()
    {
        tcpClient?.Close();
        tcpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
