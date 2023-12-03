using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.States;
using MinecraftProxy.Network.Protocol;
using MinecraftProxy.Network;
using System.Net.Sockets;
using System.Net;
using MinecraftProxy.Models;
using System.Text.Json;
using System.Security.Cryptography;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;

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

    protected readonly NetworkStream stream;

    protected MinecraftStream serverboundStream;

    protected MinecraftStream clientboundStream;

    public Player(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;

        stream = tcpClient.GetStream();
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

    public void EnableEncryption(byte[] secret)
    {
        // servers are always in offline mode - serverboundStream.EnableEncryption(secret);
        clientboundStream.EnableEncryption(secret);
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

        var buffer = new MinecraftBuffer();
        packet.Encode(buffer);

        Console.WriteLine($"Sending {packet.GetType().Name} to {direction}");

        var stream = direction switch
        {
            PacketDirection.Clientbound => clientboundStream,
            PacketDirection.Serverbound => serverboundStream,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        await stream.WritePacketAsync(id.Value, buffer);
    }

    public async Task ForwardTrafficAsync(Server server)
    {
        CurrentServer = server;

        using var forwardClient = new TcpClient(server.Host, server.Port);
        using var forwardStream = forwardClient.GetStream();

        serverboundStream = new MinecraftStream(forwardStream);
        clientboundStream = new MinecraftStream(stream);

        var completedTask = await Task.WhenAny(
                ProcessPacketsAsync<ProtocolState>(clientboundStream, serverboundStream, "Player"),
                ProcessPacketsAsync<ProtocolState>(serverboundStream, clientboundStream, "Server")
            );

        if (completedTask.IsFaulted)
            Console.WriteLine($"Unhandled exception on Player {this}:\n{completedTask.Exception}");

        Console.WriteLine($"Stopped forwarding traffic from/to {this}");
    }

    public void SetCompressionThreshold(int threshold)
    {
        serverboundStream.CompressionThreshold = threshold;
        clientboundStream.CompressionThreshold = threshold;
    }

    public override string ToString() => GameProfile?.Name ?? tcpClient.Client.RemoteEndPoint!.ToString()!;

    protected async Task ProcessPacketsAsync<T>(MinecraftStream sourceStream, MinecraftStream destinationStream, string sourceIdentifier) where T : ProtocolState
    {
        var direction = sourceIdentifier switch
        {
            "Player" => PacketDirection.Serverbound,
            "Server" => PacketDirection.Clientbound,
            _ => throw new ArgumentException(sourceIdentifier)
        };

        while (sourceStream.CanRead && sourceStream.CanWrite && destinationStream.CanRead && destinationStream.CanWrite)
        {
            var (packetId, buffer) = await sourceStream.ReadPacketAsync();

            if (packetId == -1)
                break; // Connection close

            Console.WriteLine($"{sourceIdentifier} {this} sent 0x{packetId:X2} packet");

            var (minecraftPacket, handleTask) = State switch
            {
                HandshakeState state when state.Decode<HandshakeState>(direction, packetId, buffer) is { } packet => (packet, packet.HandleAsync(state)),
                LoginState state when state.Decode<LoginState>(direction, packetId, buffer) is { } packet => (packet, packet.HandleAsync(state)),
                ConfigurationState state when state.Decode<ConfigurationState>(direction, packetId, buffer) is { } packet => (packet, packet.HandleAsync(state)),
                PlayState state when state.Decode<PlayState>(direction, packetId, buffer) is { } packet => (packet, packet.HandleAsync(state)),
                _ => ((IMinecraftPacket)null!, Task.FromResult(false))
            };

            if (await handleTask)
            {
                // cancel packet
                continue;
            }

            if (minecraftPacket is not null)
            {
                buffer.Clear();
                minecraftPacket.Encode(buffer);
            }

            await destinationStream.WritePacketAsync(packetId, buffer);
            // Console.WriteLine($"Sent 0x{packetId:X2} packet to {direction}");

            if (minecraftPacket is DisconnectPacket disconnect)
            {
                Console.WriteLine($"Player {this} disconnected from server: {disconnect.Reason}");
                break;
            }
        }
    }
}