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

    /// <summary>
    /// Finds a configured server by name using an ordinal, case-insensitive comparison.
    /// </summary>
    /// <param name="name">The server name to find.</param>
    /// <returns>The first matching server, or <see langword="null" /> when no server has that name.</returns>
    public IServer? GetByName(string name);

    /// <summary>
    /// Attempts to find a configured server by name using an ordinal, case-insensitive comparison.
    /// </summary>
    /// <param name="name">The server name to find.</param>
    /// <param name="server">When this method returns <see langword="true" />, the first matching server; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when a matching server exists; otherwise, <see langword="false" />.</returns>
    public bool TryGetByName(string name, [MaybeNullWhen(false)] out IServer server);
}
