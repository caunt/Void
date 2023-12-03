using MinecraftProxy.Network.Protocol.Forwarding;

namespace MinecraftProxy;

public class Server(string host, int port, IForwarding forwarding)
{
    public string Host { get; } = host;
    public int Port { get; } = port;
    public IForwarding Forwarding { get; } = forwarding;
}