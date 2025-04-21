using Void.Proxy.Api.Servers;

namespace Void.Proxy.Servers;

public record Server(string Name, string Host, int Port) : IServer
{
    public string? Brand { get; set; }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Name) ? Host + ":" + Port : Name;
    }
}
