namespace Void.Proxy.Api.Servers;

/// <summary>
/// Represents a server endpoint configured at runtime.
/// </summary>
/// <param name="Name">The display name used to identify the server.</param>
/// <param name="Host">The host name or address used to connect to the server.</param>
/// <param name="Port">The TCP port used to connect to the server.</param>
/// <param name="Override">An optional host name sent in place of the original client host during protocol handshakes.</param>
public record RuntimeServer(string Name, string Host, int Port, string? Override = null) : IServer
{
    /// <summary>
    /// Gets or sets the implementation brand reported by the server.
    /// </summary>
    /// <value>The reported brand, or <see langword="null" /> until a brand has been observed.</value>
    public string? Brand { get; set; }

    /// <summary>
    /// Returns the server's display name, falling back to its host and port when the name is blank.
    /// </summary>
    /// <returns><see cref="Name" /> when it contains non-whitespace text; otherwise, <c>Host:Port</c>.</returns>
    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Name) ? $"{Host}:{Port}" : Name;
    }
}
