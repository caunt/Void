namespace Void.Proxy.Api.Network.Streams.Manual;

/// <summary>
/// Defines direct byte-oriented read and write operations for a proxy stream.
/// </summary>
public interface IManualStream
{
    /// <summary>
    /// Reads up to the length of a destination span.
    /// </summary>
    /// <param name="span">The destination for bytes read from the stream.</param>
    /// <returns>The number of bytes read, which can be less than the span length.</returns>
    public int Read(Span<byte> span);

    /// <summary>
    /// Asynchronously reads up to the length of a destination memory region.
    /// </summary>
    /// <param name="memory">The destination for bytes read from the stream.</param>
    /// <param name="cancellationToken">A token used to cancel the read.</param>
    /// <returns>The number of bytes read, which can be less than the memory length.</returns>
    public ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads until a destination span is completely filled.
    /// </summary>
    /// <param name="span">The destination that must be filled.</param>
    public void ReadExactly(Span<byte> span);

    /// <summary>
    /// Asynchronously reads until a destination memory region is completely filled.
    /// </summary>
    /// <param name="memory">The destination that must be filled.</param>
    /// <param name="cancellationToken">A token used to cancel the read.</param>
    /// <returns>A task that completes when the destination is full.</returns>
    public ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes all bytes from a source span.
    /// </summary>
    /// <param name="span">The bytes to write.</param>
    public void Write(ReadOnlySpan<byte> span);

    /// <summary>
    /// Asynchronously writes all bytes from a source memory region.
    /// </summary>
    /// <param name="memory">The bytes to write.</param>
    /// <param name="cancellationToken">A token used to cancel the write.</param>
    /// <returns>A task that completes when the bytes have been written.</returns>
    public ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default);
}
