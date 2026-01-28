using System.Diagnostics.CodeAnalysis;

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

    public IServer? GetByName(string name);
    public bool TryGetByName(string name, [MaybeNullWhen(false)] out IServer server);
}
