using System.Net.Sockets;

namespace Void.Proxy.Api.Servers;

public interface IServer
{
    public string Name { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string? Brand { get; set; }

    public TcpClient CreateTcpClient()
    {
        return new TcpClient(Host, Port);
    }
}