using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Encryption;

/// <summary>
/// Requests the private key used to decrypt an encrypted connection to a destination server.
/// </summary>
/// <param name="Server">The destination server whose private key is requested.</param>
public record SearchServerPrivateKeyEvent(IServer Server) : IEventWithResult<byte[]>
{
    /// <summary>
    /// Gets or sets the encoded private key supplied by a listener.
    /// </summary>
    /// <value>The private-key bytes, or <see langword="null" /> when no key is available for the server.</value>
    public byte[]? Result { get; set; }
}
