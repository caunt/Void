namespace Void.Proxy.Api.Events.Authentication;

/// <summary>
/// Identifies which side of a proxied connection is being authenticated.
/// </summary>
public enum AuthenticationSide
{
    /// <summary>
    /// The proxy is authenticating its connection to the destination server.
    /// </summary>
    Server,

    /// <summary>
    /// The proxy is authenticating the connecting player.
    /// </summary>
    Proxy
}
