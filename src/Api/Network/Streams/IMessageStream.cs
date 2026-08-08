namespace Void.Proxy.Api.Network.Streams;

/// <summary>
/// Represents a message-processing stream that can wrap another stream layer.
/// </summary>
public interface IMessageStream : IMessageStreamBase
{
    /// <summary>
    /// Gets or sets the stream layer wrapped by this message stream.
    /// </summary>
    /// <value>The wrapped stream, or <see langword="null" /> when no base layer is attached.</value>
    public IMessageStreamBase? BaseStream { get; set; }
}
