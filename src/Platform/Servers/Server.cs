using Void.Proxy.Api.Servers;

namespace Void.Proxy.Servers;

public class Server(string name, string host, int port) : IServer
{
    public string? Brand { get; set; }
    public string Name { get; set; } = name;
    public string Host { get; set; } = host;
    public int Port { get; set; } = port;

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Name) ? Host + ":" + Port : Name;
    }
}
