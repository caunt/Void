using System.Net.Sockets;

namespace Void.Proxy.Api.Servers;

/// <summary>
/// Represents a server that players can connect to.
/// </summary>
public interface IServer
{
    /// <summary>
    /// The default Minecraft server port.
    /// </summary>
    public const int DefaultPort = 25565;
    /// <summary>
    /// Gets the name of the server.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the host address of the server.
    /// </summary>
    public string Host { get; }

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
    public TcpClient CreateTcpClient() => new(Host, Port);
}
