using System.Net;
using System.Net.Sockets;
using Void.Proxy.Api.Configurations.Attributes;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Configurations;

[RootConfiguration("settings")]
public record Settings : ISettings
{
    public string Address { get; init; } = (Socket.OSSupportsIPv6 ? IPAddress.IPv6Any : IPAddress.Any).ToString();

    public List<RuntimeServer> Servers { get; init; } =
    [
        new(Name: "lobby", Host: "127.0.0.1", Port: 25566, Override: "lobby.example.org"),
        new(Name: "minigames", Host: "127.0.0.1", Port: 25567),
        new(Name: "limbo", Host: "127.0.0.1", Port: 25568)
    ];

    public int Port { get; init; } = 25565;
    public int CompressionThreshold { get; init; } = 256;
    public int KickTimeout { get; init; } = 10_000;
    public LogLevel LogLevel { get; init; } = LogLevel.Information;
    public bool Offline { get; set; } = false;

    IPAddress ISettings.Address => IPAddress.Parse(Address);
    IEnumerable<IServer> ISettings.Servers => Servers;
}
