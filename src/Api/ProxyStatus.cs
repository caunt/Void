namespace Void.Proxy.Api;

/// <summary>
/// Represents the operational state of the proxy server.
/// </summary>
public enum ProxyStatus
{
    /// <summary>
    /// The proxy is running and accepting connections.
    /// </summary>
    Alive,

    /// <summary>
    /// The proxy has temporarily paused accepting new connections.
    /// </summary>
    Paused,

    /// <summary>
    /// The proxy is in the process of shutting down.
    /// </summary>
    Stopping
}
