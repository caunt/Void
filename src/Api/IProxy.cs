using Microsoft.Extensions.Hosting;

namespace Void.Proxy.Api;

public interface IProxy : IHostedService
{
    /// <summary>
    /// Starts accepting connections on the listener.
    /// </summary>
    public void StartAcceptingConnections();

    /// <summary>
    /// Pauses the acceptance of new incoming connections.
    /// </summary>
    /// <param name="waitOnlinePlayers">Specifies whether to wait for online players to disconnect if proxy is shutting down.</param>
    /// <remarks>This method temporarily halts the ability to accept new connections. Existing connections
    /// remain unaffected and continue to operate normally. Use this method when you need to stop accepting new
    /// connections without disrupting online players.</remarks>
    public void PauseAcceptingConnections(bool waitOnlinePlayers = false);
}
