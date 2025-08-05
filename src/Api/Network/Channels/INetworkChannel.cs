using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Network.Streams;

namespace Void.Proxy.Api.Network.Channels;

/// <summary>
/// Represents a network channel that manages a pipeline of <see cref="IMessageStream"/> instances.
/// </summary>
public interface INetworkChannel : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the channel can read messages.
    /// </summary>
    public bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the channel can write messages.
    /// </summary>
    public bool CanWrite { get; }

    /// <summary>
    /// Gets the first stream in the channel's pipeline.
    /// </summary>
    public IMessageStreamBase Head { get; }

    /// <summary>
    /// Gets a value indicating whether the channel is active.
    /// </summary>
    public bool IsAlive { get; }

    /// <summary>
    /// Gets a value indicating whether the channel has been configured.
    /// </summary>
    public bool IsConfigured { get; }

    /// <summary>
    /// Gets a value indicating whether the channel is currently paused.
    /// </summary>
    public bool IsPaused { get; }

    /// <summary>
    /// Adds a new stream of type <typeparamref name="T"/> to the end of the pipeline.
    /// </summary>
    public void Add<T>() where T : class, IMessageStream, new();

    /// <summary>
    /// Adds the specified <paramref name="stream"/> to the end of the pipeline.
    /// </summary>
    public void Add<T>(T stream) where T : class, IMessageStream;

    /// <summary>
    /// Inserts a new stream of type <typeparamref name="TValue"/> before <typeparamref name="TBefore"/>.
    /// </summary>
    public void AddBefore<TBefore, TValue>() where TBefore : class, IMessageStream where TValue : class, IMessageStream, new();

    /// <summary>
    /// Inserts the specified <paramref name="stream"/> before <typeparamref name="TBefore"/>.
    /// </summary>
    public void AddBefore<TBefore, TValue>(TValue stream) where TBefore : class, IMessageStream where TValue : class, IMessageStream;
    /// <summary>
    /// Removes a stream of type <typeparamref name="T"/> from the pipeline.
    /// </summary>
    public void Remove<T>() where T : class, IMessageStream;

    /// <summary>
    /// Removes the specified <paramref name="stream"/> from the pipeline.
    /// </summary>
    public void Remove<T>(T stream) where T : class, IMessageStream;

    /// <summary>
    /// Gets the stream of type <typeparamref name="T"/>.
    /// </summary>
    public T Get<T>() where T : class, IMessageStreamBase;

    /// <summary>
    /// Determines whether a stream of type <typeparamref name="T"/> exists in the pipeline.
    /// </summary>
    public bool Has<T>() where T : class, IMessageStreamBase;

    /// <summary>
    /// Attempts to retrieve a stream of type <typeparamref name="T"/>.
    /// </summary>
    public bool TryGet<T>([MaybeNullWhen(false)] out T result) where T : class, IMessageStreamBase;
    /// <summary>
    /// Inserts the given <paramref name="memory"/> at the start of the buffer.
    /// </summary>
    public void PrependBuffer(Memory<byte> memory);

    /// <summary>
    /// Reads the next message from the channel.
    /// </summary>
    public ValueTask<INetworkMessage> ReadMessageAsync(Side origin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the specified <paramref name="message"/> to the channel.
    /// </summary>
    public ValueTask WriteMessageAsync(INetworkMessage message, Side origin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses channel operations.
    /// </summary>
    public void Pause(Operation operation = Operation.Read);

    /// <summary>
    /// Attempts to pause channel operations.
    /// </summary>
    public bool TryPause(Operation operation = Operation.Read);

    /// <summary>
    /// Resumes channel operations.
    /// </summary>
    public void Resume(Operation operation = Operation.Read);

    /// <summary>
    /// Attempts to resume channel operations.
    /// </summary>
    public bool TryResume(Operation operation = Operation.Read);

    /// <summary>
    /// Flushes any buffered data.
    /// </summary>
    public void Flush();

    /// <summary>
    /// Asynchronously flushes any buffered data.
    /// </summary>
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the channel and releases resources.
    /// </summary>
    public void Close();
}
