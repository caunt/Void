using System.Net;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;
using Void.Proxy.Servers;

namespace Void.Proxy.Configurations;

public record Settings : ISettings
{
    public IPAddress Address { get; set; } = IPAddress.Any;
    public int Port { get; set; } = 25565;
    public int CompressionThreshold { get; set; } = 256;
    public int KickTimeout { get; set; } = 10_000;
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public List<IServer> Servers { get; set; } =
    [
        new Server("lobby", "127.0.0.1", 25566),
        new Server("minigames", "127.0.0.1", 25567),
        new Server("limbo", "127.0.0.1", 25568),
    ];
}
