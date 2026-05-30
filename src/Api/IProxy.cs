namespace Void.Proxy.Api;

/// <summary>
/// Represents the running proxy instance and exposes listener state and lifecycle control for API consumers.
/// </summary>
/// <remarks>
/// <para>
/// The proxy owns the TCP listener that accepts Minecraft client connections. <see cref="Status"/> reports the
/// externally visible lifecycle state exposed to plugins and host components.
/// </para>
/// <para>
/// <see cref="StartAcceptingConnectionsAsync"/> and <see cref="PauseAcceptingConnections"/> control acceptance of
/// new connections. Pausing stops the listener from accepting new clients but does not disconnect already linked
/// players. <see cref="Stop"/> requests host shutdown; when player draining is requested, the shutdown request is
/// deferred until the current player collection becomes empty.
/// </para>
/// </remarks>
/// <seealso cref="ProxyStatus"/>
public interface IProxy
{
    /// <summary>
    /// Gets a value indicating the current status of the proxy.
    /// </summary>
    public ProxyStatus Status { get; }

    /// <summary>
    /// Starts accepting connections on the listener.
    /// </summary>
    public ValueTask StartAcceptingConnectionsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Pauses the acceptance of new incoming connections.
    /// </summary>
    /// <remarks>This method temporarily halts the ability to accept new connections. Existing connections
    /// remain unaffected and continue to operate normally. Use this method when you need to stop accepting new
    /// connections without disrupting online players.</remarks>
    public void PauseAcceptingConnections();

    /// <summary>
    /// Stops the server and optionally waits for all online players to disconnect.
    /// </summary>
    /// <remarks>If <paramref name="waitOnlinePlayers"/> is set to <see langword="true"/>, the proxy will
    /// wait until all players have disconnected. Use this option to ensure a graceful shutdown when active player
    /// sessions are present.</remarks>
    /// <param name="waitOnlinePlayers">A value indicating whether to wait for all online players to disconnect before stopping the server. <see langword="true"/> to wait for players to disconnect; otherwise, <see langword="false"/>.</param>
    public void Stop(bool waitOnlinePlayers = false);
}
