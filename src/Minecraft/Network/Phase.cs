namespace Void.Minecraft.Network;

/// <summary>
/// Identifies the current state of a Minecraft Java Edition protocol connection.
/// </summary>
public enum Phase
{
    /// <summary>
    /// The initial state in which the client selects status or login intent.
    /// </summary>
    Handshake,

    /// <summary>
    /// The server-list status and ping state.
    /// </summary>
    Status,

    /// <summary>
    /// The authentication and profile-login state.
    /// </summary>
    Login,

    /// <summary>
    /// The post-login configuration state introduced in protocol 1.20.2.
    /// </summary>
    Configuration,

    /// <summary>
    /// The active gameplay state.
    /// </summary>
    Play
}
