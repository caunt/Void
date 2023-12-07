using MinecraftProxy.Models;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.States;
using MinecraftProxy.Network.Protocol.States.Common;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;

namespace MinecraftProxy;

public class Player
{
    public GameProfile? GameProfile { get; protected set; }

    public ProtocolState State { get; protected set; }

    public EndPoint? RemoteEndPoint => tcpClient.Client.RemoteEndPoint;

    public Server CurrentServer { get; protected set; }

    public IdentifiedKey IdentifiedKey { get; protected set; }

    public Guid SignatureHolder { get; protected set; }

    protected readonly TcpClient tcpClient;

    protected MinecraftChannel serverChannel;

    protected MinecraftChannel clientChannel;

    public Player(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;
        SwitchState(0);
    }

    public void SetGameProfile(GameProfile gameProfile)
    {
        GameProfile = gameProfile;
    }

    public void SwitchState(int state)
    {
        State = state switch
        {
            0 => new HandshakeState(this),
            2 => new LoginState(this),
            3 => new ConfigurationState(this),
            4 => new PlayState(this),
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };

        Console.WriteLine($"Player {this} switch state to {State.GetType().Name}");
    }

    public void EnableEncryption(PacketDirection direction, byte[] secret)
    {
        var channel = direction switch
        {
            PacketDirection.Clientbound => clientChannel,
            PacketDirection.Serverbound => serverChannel, // servers are always in offline mode
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        channel.EnableEncryption(secret);
        Console.WriteLine($"Player {this} enabled {direction} encryption");
    }

    public void EnableCompression(PacketDirection direction, int threshold)
    {
        var channel = direction switch
        {
            PacketDirection.Clientbound => clientChannel,
            PacketDirection.Serverbound => serverChannel,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        channel.EnableCompression(threshold);
        Console.WriteLine($"Player {this} enabled {direction} compression");
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

        GameProfile = JsonSerializer.Deserialize<GameProfile>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return GameProfile;
    }

    public async Task SendPacketAsync(PacketDirection direction, IMinecraftPacket packet)
    {
        var id = State.FindPacketId(direction, packet);

        if (!id.HasValue)
            throw new Exception($"{packet.GetType().Name} packet id not found in {State.GetType().Name}");

        Console.WriteLine($"Sending {packet.GetType().Name} to {direction}");

        var channel = direction switch
        {
            PacketDirection.Clientbound => clientChannel,
            PacketDirection.Serverbound => serverChannel,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        using var message = EncodeMessage(id.Value, packet, direction);
        await channel.WriteMessageAsync(message);
    }

    public async Task ForwardTrafficAsync(Server server)
    {
        CurrentServer = server;

        using var forwardClient = new TcpClient(server.Host, server.Port);
        using var forwardStream = forwardClient.GetStream();

        serverChannel = new MinecraftChannel(forwardStream);
        clientChannel = new MinecraftChannel(tcpClient.GetStream());

        var cts = new CancellationTokenSource();

        var clientTask = ProcessPacketsAsync<ProtocolState>(clientChannel, serverChannel, "Player", cts.Token);
        var serverTask = ProcessPacketsAsync<ProtocolState>(serverChannel, clientChannel, "Server", cts.Token);
        var completedTask = await Task.WhenAny(serverTask, clientTask);

        cts.Cancel();

        if (completedTask.IsFaulted && completedTask.Exception.InnerExceptions.All(exception => exception is not EndOfStreamException and not IOException))
            Console.WriteLine($"Unhandled exception while reading {(completedTask == serverTask ? "Server" : "Client")} channel ({this}):\n{completedTask.Exception}");

        // graceful opposite disconnection timeout
        var timeout = Task.Delay(5000);
        var disconnectTask = await Task.WhenAny(completedTask == serverTask ? clientTask : serverTask, timeout);

        if (disconnectTask == timeout)
            Console.WriteLine($"Timed out waiting {(completedTask == serverTask ? "Client" : "Server")} disconnection");

        Console.WriteLine($"Stopped forwarding traffic from/to {this}");

        forwardClient.Close();
        tcpClient.Close();
    }

    public override string ToString() => GameProfile?.Name ?? tcpClient.Client?.RemoteEndPoint?.ToString() ?? "Disposed?";

    protected async Task ProcessPacketsAsync<T>(MinecraftChannel sourceChannel, MinecraftChannel destinationChannel, string sourceIdentifier, CancellationToken cancellationToken) where T : ProtocolState
    {
        var direction = sourceIdentifier switch
        {
            "Player" => PacketDirection.Serverbound,
            "Server" => PacketDirection.Clientbound,
            _ => throw new ArgumentException(sourceIdentifier)
        };

        while (sourceChannel.CanRead && sourceChannel.CanWrite && destinationChannel.CanRead && destinationChannel.CanWrite)
        {
            int packetId;
            IMinecraftPacket? packet;

            using (var message = await sourceChannel.ReadMessageAsync(cancellationToken))
            {
                (packetId, packet, var handleTask) = DecodeMessage(State, message, direction);

                if (packet is null)
                {
                    await destinationChannel.WriteMessageAsync(message, cancellationToken);
                    continue;
                }

                if (await handleTask)
                    continue;
            }

            using (var message = EncodeMessage(packetId, packet, direction))
            {
                await destinationChannel.WriteMessageAsync(message, cancellationToken);
            }

            if (packet is DisconnectPacket disconnect)
            {
                Console.WriteLine($"Player {this} disconnected from server: {disconnect.Reason}");
                break;
            }
        }
    }

    protected (int, IMinecraftPacket?, Task<bool>) DecodeMessage(ProtocolState protocolState, MinecraftMessage message, PacketDirection direction)
    {
        var buffer = new MinecraftBuffer(message.Memory);
        var packetId = message.PacketId;
        Console.WriteLine($"Decoding {direction} 0x{packetId:X2} packet");

        try
        {
            return protocolState switch
            {
                HandshakeState state when state.Decode<HandshakeState>(packetId, direction, ref buffer) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                LoginState state when state.Decode<LoginState>(packetId, direction, ref buffer) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                ConfigurationState state when state.Decode<ConfigurationState>(packetId, direction, ref buffer) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                PlayState state when state.Decode<PlayState>(packetId, direction, ref buffer) is { } packet => (packetId, packet, packet.HandleAsync(state)),
                _ => (packetId, null, Task.FromResult(false))
            };
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Couldn't decode packet: {exception}");
            return (-1, null, Task.FromResult(false));
        }
    }

    protected static MinecraftMessage EncodeMessage(int packetId, IMinecraftPacket packet, PacketDirection direction)
    {
        var memoryOwner = MemoryPool<byte>.Shared.Rent(2048);
        var buffer = new MinecraftBuffer(memoryOwner.Memory);
        Console.WriteLine($"Encoding {direction} 0x{packetId:X2} packet {JsonSerializer.Serialize(packet as object, new JsonSerializerOptions { WriteIndented = true })}");

        packet.Encode(ref buffer);

        return new(packetId, memoryOwner.Memory[..buffer.Position], memoryOwner);
    }
}