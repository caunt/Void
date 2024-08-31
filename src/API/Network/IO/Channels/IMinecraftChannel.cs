using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams;

namespace Void.Proxy.API.Network.IO.Channels;

public interface IMinecraftChannel : IDisposable, IAsyncDisposable
{
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public IMinecraftStreamBase Head { get; }
    public bool IsConfigured { get; }
    public bool IsRedirectionSupported { get; }

    public void Add<T>() where T : IMinecraftStream, new();
    public void Add<T>(T stream) where T : IMinecraftStream;
    public void AddBefore<TBefore, TValue>() where TBefore : IMinecraftStream where TValue : IMinecraftStream, new();
    public void AddBefore<TBefore, TValue>(TValue stream) where TBefore : IMinecraftStream where TValue : IMinecraftStream;
    public T Get<T>() where T : IMinecraftStreamBase;
    public void PrependBuffer(Memory<byte> memory);
    public ValueTask<IMinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default);
    public ValueTask WriteMessageAsync(IMinecraftMessage message, CancellationToken cancellationToken = default);
    public void Flush();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
    public void Close();
}