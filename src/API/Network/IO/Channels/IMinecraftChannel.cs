using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams;

namespace Void.Proxy.API.Network.IO.Channels;

public interface IMinecraftChannel : IDisposable, IAsyncDisposable
{
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public IMinecraftStreamBase Head { get; }

    public bool IsConfigured { get; }
    public bool IsPaused { get; }

    public void Add<T>() where T : class, IMinecraftStream, new();
    public void Add<T>(T stream) where T : class, IMinecraftStream;
    public void AddBefore<TBefore, TValue>() where TBefore : class, IMinecraftStream where TValue : class, IMinecraftStream, new();
    public void AddBefore<TBefore, TValue>(TValue stream) where TBefore : class, IMinecraftStream where TValue : class, IMinecraftStream;
    public void Remove<T>() where T : class, IMinecraftStream, new();
    public void Remove<T>(T stream) where T : class, IMinecraftStream;
    public T Get<T>() where T : class, IMinecraftStreamBase;
    public bool Search<T>([MaybeNullWhen(false)] out T result) where T : class, IMinecraftStreamBase;
    public void PrependBuffer(Memory<byte> memory);
    public ValueTask<IMinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default);
    public ValueTask WriteMessageAsync(IMinecraftMessage message, CancellationToken cancellationToken = default);
    public void Pause();
    public void Resume();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
    public void Close();
}