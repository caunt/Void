namespace Void.Proxy.Api.Servers;

/// <summary>
/// Provides access to the collection of servers available to the proxy.
/// </summary>
public interface IServerService
{
    /// <summary>
    /// Gets all configured servers.
    /// </summary>
    public IEnumerable<IServer> All { get; }
}
