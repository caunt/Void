using MinecraftProxy.Network.Protocol.Forwarding;

namespace MinecraftProxy.Models;

public class ServerInfo(string host, int port, IForwarding forwarding)
{
    public string Host { get; } = host;
    public int Port { get; } = port;
    public IForwarding Forwarding { get; } = forwarding;
}