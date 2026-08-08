namespace Void.Proxy.Api.Network;

/// <summary>
/// Identifies the protocol direction in which a network message travels.
/// </summary>
[Flags]
public enum Direction
{
    /// <summary>
    /// Traffic traveling toward the player client. This member has the underlying value <c>0</c>.
    /// </summary>
    Clientbound,

    /// <summary>
    /// Traffic traveling toward the destination server.
    /// </summary>
    Serverbound
}
