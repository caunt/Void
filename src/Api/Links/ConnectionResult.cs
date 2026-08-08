namespace Void.Proxy.Api.Links;

/// <summary>
/// Describes whether an attempt to connect a player to a server succeeded.
/// </summary>
public enum ConnectionResult
{
    /// <summary>
    /// No server link was established.
    /// </summary>
    NotConnected,

    /// <summary>
    /// A server link was established.
    /// </summary>
    Connected
}
