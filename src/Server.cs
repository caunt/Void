using MinecraftProxy.Network;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.Forwarding;
using MinecraftProxy.Network.Protocol.Packets;
using System.Net.Sockets;

namespace MinecraftProxy;

public class Server(string host, int port, IForwarding forwarding) : IDisposable
{
    public string Host { get; } = host;
    public int Port { get; } = port;
    public IForwarding Forwarding { get; } = forwarding;
    public string? Brand { get; protected set; }

    protected MinecraftChannel? channel;
    protected TcpClient? tcpClient;

    public Server Init()
    {
        tcpClient = new TcpClient(Host, Port);
        channel = new MinecraftChannel(tcpClient.GetStream());

        return this;
    }

    public MinecraftChannel GetChannel()
    {
        return channel ?? throw new InvalidOperationException("Server not initialized yet");
    }

    public void SetBrand(string brand)
    {
        Brand = brand;
    }

    public void EnableEncryption(byte[] secret, bool force = false)
    {
        if (!force)
            throw new NotSupportedException("Servers are always in offline mode, encryption doesn't work with it. Specify force to continue anyway.");

        ArgumentNullException.ThrowIfNull(channel);

        channel.EnableEncryption(secret);
        Proxy.Logger.Information($"Server {this} enabled encryption");
    }

    public void EnableCompression(int threshold)
    {
        ArgumentNullException.ThrowIfNull(channel);

        channel.EnableCompression(threshold);
        Proxy.Logger.Information($"Server {this} enabled compression");
    }

    public async Task SendPacketAsync(Player from, IMinecraftPacket packet)
    {
        ArgumentNullException.ThrowIfNull(from.State);
        ArgumentNullException.ThrowIfNull(from.ProtocolVersion);

        var id = from.State.FindPacketId(Direction.Serverbound, packet, from.ProtocolVersion);

        if (!id.HasValue)
            throw new Exception($"{packet.GetType().Name} packet id not found in player {from.State.GetType().Name}");

        Proxy.Logger.Debug($"Sending {packet.GetType().Name} to server {this}");

        ArgumentNullException.ThrowIfNull(channel);

        using var message = MinecraftMessage.Encode(id.Value, packet, Direction.Serverbound, from.ProtocolVersion);
        await channel.WriteMessageAsync(message);
    }

    public void Dispose()
    {
        tcpClient?.Close();
        tcpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}