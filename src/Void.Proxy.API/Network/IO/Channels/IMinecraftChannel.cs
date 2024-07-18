using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams;

namespace Void.Proxy.API.Network.IO.Channels;

public interface IMinecraftChannel : IDisposable, IAsyncDisposable
{
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public IMinecraftStreamBase Head { get; }
    public bool IsConfigured { get; }

    public void Add<T>() where T : IMinecraftStream, new();
    public void PrependBuffer(Memory<byte> memory);
    public ValueTask<IMinecraftMessage> ReadMessageAsync();
    public ValueTask WriteMessageAsync(IMinecraftMessage message);
    public void Flush();
    public ValueTask FlushAsync();
    public void Close();
}