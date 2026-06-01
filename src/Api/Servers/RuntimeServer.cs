namespace Void.Proxy.Api.Servers;

public record RuntimeServer(string Name, string Host, int Port, string? Override = null) : IServer
{
    public string? Brand { get; set; }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Name) ? $"{Host}:{Port}" : Name;
    }
}
