namespace Void.Proxy.Api.Links;

public enum LinkStopReason
{
    /// <summary>
    /// Specifies that the player disconnected from the proxy.
    /// </summary>
    PlayerDisconnected,

    /// <summary>
    /// Specifies that the player was kicked from the server or the server closed the connection abnormally.
    /// </summary>
    ServerDisconnected,

    /// <summary>
    /// Internal exception occurred in <see cref="ILink"/> implementation.
    /// </summary>
    InternalException,

    /// <summary>
    /// Manually requested to stop the <see cref="ILink"/> implementation, e.g., by calling the <see cref="ILink.StopAsync"/> method.
    /// </summary>
    Requested
}
