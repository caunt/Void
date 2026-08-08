using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Api.Network.Streams.Manual.Binary;

/// <summary>
/// Defines framed I/O operations in which each read or write handles one complete binary message.
/// </summary>
public interface ICompleteMessageStream : IMessageStream
{
    /// <summary>
    /// Reads the next complete message from the stream.
    /// </summary>
    /// <returns>The next complete message. The caller owns and must dispose the returned message.</returns>
    public ICompleteBinaryMessage ReadMessage();

    /// <summary>
    /// Asynchronously reads the next complete message from the stream.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the read.</param>
    /// <returns>The next complete message. The caller owns and must dispose the returned message.</returns>
    public ValueTask<ICompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes one complete message to the stream.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public void WriteMessage(ICompleteBinaryMessage message);

    /// <summary>
    /// Asynchronously writes one complete message to the stream.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="cancellationToken">A token used to cancel the write.</param>
    /// <returns>A task that completes when the message has been written.</returns>
    public ValueTask WriteMessageAsync(ICompleteBinaryMessage message, CancellationToken cancellationToken = default);
}
