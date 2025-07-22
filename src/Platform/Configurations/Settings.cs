using System.Net;
using Void.Proxy.Api.Configurations.Attributes;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;
using Void.Proxy.Servers;

namespace Void.Proxy.Configurations;

[RootConfiguration("settings")]
public record Settings : ISettings
{
    public string Address { get; init; } = IPAddress.Any.ToString();
    public int Port { get; init; } = 25565;
    public int CompressionThreshold { get; init; } = 256;
    public int KickTimeout { get; init; } = 10_000;
    public LogLevel LogLevel { get; init; } = LogLevel.Information;
    public bool Offline { get; init; } = false;
    public List<Server> Servers { get; init; } =
    [
        new("lobby", "127.0.0.1", 25566),
        new("minigames", "127.0.0.1", 25567),
        new("limbo", "127.0.0.1", 25568)
    ];

    IPAddress ISettings.Address => IPAddress.Parse(Address);
    IEnumerable<IServer> ISettings.Servers => Servers;
    bool ISettings.Offline => Offline;
}
