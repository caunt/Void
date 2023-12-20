using System.Net.Sockets;
using Void.Proxy.Network.Protocol.Forwarding;

namespace Void.Proxy.Models.General;

public class ServerInfo(string name, string host, int port, IForwarding forwarding)
{
    public string Name { get; } = name;
    public string Host { get; } = host;
    public int Port { get; } = port;
    public IForwarding Forwarding { get; } = forwarding;

    public TcpClient CreateTcpClient() => new(Host, Port);
}