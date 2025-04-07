using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Void.Common.Network.Messages;
using Void.Common.Network.Streams;

namespace Void.Common.Network.Channels;

public interface INetworkChannel : IDisposable, IAsyncDisposable
{
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public INetworkStreamBase Head { get; }

    public bool IsAlive { get; }
    public bool IsConfigured { get; }
    public bool IsPaused { get; }

    public void Add<T>() where T : class, INetworkStream, new();
    public void Add<T>(T stream) where T : class, INetworkStream;
    public void AddBefore<TBefore, TValue>() where TBefore : class, INetworkStream where TValue : class, INetworkStream, new();
    public void AddBefore<TBefore, TValue>(TValue stream) where TBefore : class, INetworkStream where TValue : class, INetworkStream;
    public void Remove<T>() where T : class, INetworkStream, new();
    public void Remove<T>(T stream) where T : class, INetworkStream;
    public T Get<T>() where T : class, INetworkStreamBase;
    public bool Has<T>() where T : class, INetworkStreamBase;
    public bool TryGet<T>([MaybeNullWhen(false)] out T result) where T : class, INetworkStreamBase;
    public void PrependBuffer(Memory<byte> memory);
    public ValueTask<INetworkMessage> ReadMessageAsync(CancellationToken cancellationToken = default);
    public ValueTask WriteMessageAsync(INetworkMessage message, Side origin, CancellationToken cancellationToken = default);
    public void Pause(Operation operation = Operation.Read);
    public bool TryPause(Operation operation = Operation.Read);
    public void Resume(Operation operation = Operation.Read);
    public bool TryResume(Operation operation = Operation.Read);
    public void Flush();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
    public void Close();
}
