using System.Net;
using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Settings;

/// <summary>
/// Exposes the runtime-readable configuration values that govern core proxy behavior,
/// including network binding, protocol settings, and the backend server list.
/// </summary>
public interface ISettings
{
    /// <summary>
    /// Gets the network address the proxy binds to when accepting incoming player connections.
    /// Defaults to <see cref="IPAddress.Any"/>, which listens on all available network interfaces.
    /// </summary>
    public IPAddress Address { get; }

    /// <summary>
    /// Gets the TCP port the proxy listens on for incoming player connections.
    /// Defaults to <c>25565</c>, the standard Minecraft Java Edition port.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets the minimum uncompressed packet payload size in bytes at which Zlib compression is applied.
    /// Packets whose payload is smaller than this threshold are forwarded without compression.
    /// Defaults to <c>256</c> bytes.
    /// </summary>
    public int CompressionThreshold { get; }

    /// <summary>
    /// Gets the maximum time in milliseconds the proxy waits for plugins to finish graceful kick
    /// handling before force-disconnecting the player.
    /// Defaults to <c>10000</c> ms (10 seconds).
    /// </summary>
    public int KickTimeout { get; }

    /// <summary>
    /// Gets the minimum severity level for log messages emitted by the proxy.
    /// Defaults to <see cref="LogLevel.Information"/>.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the proxy operates in offline mode.
    /// When <see langword="true"/>, Mojang authentication is bypassed and players can connect
    /// without a valid premium account.
    /// </summary>
    /// <remarks>
    /// This flag can be overridden at startup via the <c>VOID_OFFLINE</c> environment variable
    /// or a command-line option.
    /// </remarks>
    public bool Offline { get; set; }

    /// <summary>
    /// Gets the collection of backend Minecraft servers the proxy is configured to route players to.
    /// </summary>
    public IEnumerable<IServer> Servers { get; }
}
