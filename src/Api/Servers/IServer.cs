using System.Net.Sockets;

namespace Void.Proxy.Api.Servers;

/// <summary>
/// Represents a server that players can connect to.
/// </summary>
public interface IServer
{
    /// <summary>
    /// Gets the name of the server.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the host address of the server.
    /// </summary>
    public string Host { get; }

    /// <summary>
    /// Gets or sets the hostname override for the server.
    /// </summary>
    public string? Override { get; }

    /// <summary>
    /// Gets the port used for connecting to the server.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets or sets the implementation brand of the server.
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Creates a <see cref="TcpClient"/> connected to this server.
    /// </summary>
    /// <returns>A configured <see cref="TcpClient"/> instance.</returns>
    public async Task<TcpClient> CreateTcpClientAsync(CancellationToken cancellationToken = default)
    {
        var client = new TcpClient();
        await client.ConnectAsync(Host, Port, cancellationToken);

        return client;
    }
}
