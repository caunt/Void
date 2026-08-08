using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Api.Network.Streams.Manual.Binary;

/// <summary>
/// Defines byte-oriented I/O with helpers that expose buffered data as binary messages.
/// </summary>
public interface IBufferedMessageStream : IManualStream, IMessageStream
{
    /// <summary>
    /// Reads available data into a buffered message, subject to a maximum size.
    /// </summary>
    /// <param name="maxSize">The maximum number of bytes to place in the returned message.</param>
    /// <returns>A buffered message owned by the caller.</returns>
    public IBufferedBinaryMessage ReadAsMessage(int maxSize = 2048);

    /// <summary>
    /// Asynchronously reads available data into a buffered message, subject to a maximum size.
    /// </summary>
    /// <param name="maxSize">The maximum number of bytes to place in the returned message.</param>
    /// <param name="cancellationToken">A token used to cancel the read.</param>
    /// <returns>A buffered message owned by the caller.</returns>
    public ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the buffered bytes contained in a message.
    /// </summary>
    /// <param name="message">The message whose buffered bytes are written.</param>
    public void WriteAsMessage(IBufferedBinaryMessage message);

    /// <summary>
    /// Asynchronously writes the buffered bytes contained in a message.
    /// </summary>
    /// <param name="message">The message whose buffered bytes are written.</param>
    /// <param name="cancellationToken">A token used to cancel the write.</param>
    /// <returns>A task that completes when the buffered bytes have been written.</returns>
    public ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default);
}
