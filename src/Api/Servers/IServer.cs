using System.Net.Sockets;

namespace Void.Proxy.Api.Servers;

public interface IServer
{
    public string Name { get; }
    public string Host { get; }
    public int Port { get; }
    public string? Brand { get; set; }

    public TcpClient CreateTcpClient() => new(Host, Port);
}
