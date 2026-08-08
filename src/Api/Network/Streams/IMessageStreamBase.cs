namespace Void.Proxy.Api.Network.Streams;

/// <summary>
/// Defines lifecycle and flushing operations shared by proxy message streams.
/// </summary>
public interface IMessageStreamBase : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets whether the stream currently supports reading.
    /// </summary>
    public bool CanRead { get; }

    /// <summary>
    /// Gets whether the stream currently supports writing.
    /// </summary>
    public bool CanWrite { get; }

    /// <summary>
    /// Gets whether the stream remains open and usable.
    /// </summary>
    public bool IsAlive { get; }

    /// <summary>
    /// Flushes buffered output to the underlying stream.
    /// </summary>
    public void Flush();

    /// <summary>
    /// Asynchronously flushes buffered output to the underlying stream.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the flush.</param>
    /// <returns>A task that completes when buffered output has been flushed.</returns>
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the stream and prevents further I/O.
    /// </summary>
    public void Close();
}
