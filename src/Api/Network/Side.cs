namespace Void.Proxy.Api.Network;

/// <summary>
/// Identifies an endpoint participating in proxy message processing.
/// </summary>
public enum Side
{
    /// <summary>
    /// The proxy's internal processing endpoint.
    /// </summary>
    Proxy,

    /// <summary>
    /// The destination Minecraft server.
    /// </summary>
    Server,

    /// <summary>
    /// The connected player client.
    /// </summary>
    Client
}
