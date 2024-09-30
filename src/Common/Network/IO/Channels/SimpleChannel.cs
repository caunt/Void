using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.IO.Streams.Manual.Binary;
using Void.Proxy.API.Network.IO.Streams.Manual.Network;
using Void.Proxy.Common.Network.IO.Messages;
using Void.Proxy.Common.Network.IO.Messages.Binary;
using Void.Proxy.Common.Network.IO.Streams.Packet;

namespace Void.Proxy.Common.Network.IO.Channels;

public class SimpleChannel(IMinecraftStreamBase head) : IMinecraftChannel
{
    private TaskCompletionSource? _pause;

    public bool CanRead => true;
    public bool CanWrite => true;

    public IMinecraftStreamBase Head => head;

    public bool IsConfigured => head is IMinecraftStream;
    public bool IsPaused => _pause is { Task.IsCompleted: false };
    public bool IsRedirectionSupported => false;

    public void Add<T>() where T : class, IMinecraftStream, new()
    {
        Add(new T());
    }

    public void Add<T>(T stream) where T : class, IMinecraftStream
    {
        stream.BaseStream = head;
        head = stream;
    }

    public void AddBefore<TBefore, TValue>() where TBefore : class, IMinecraftStream where TValue : class, IMinecraftStream, new()
    {
        AddBefore<TBefore, TValue>(new TValue());
    }

    public void AddBefore<TBefore, TValue>(TValue stream) where TBefore : class, IMinecraftStream where TValue : class, IMinecraftStream
    {
        var before = Get<TBefore>();
        var beforeBaseStream = before.BaseStream;

        stream.BaseStream = beforeBaseStream;
        before.BaseStream = stream;
    }

    public void Remove<T>() where T : class, IMinecraftStream, new()
    {
        Remove(Get<T>());
    }

    public void Remove<T>(T value) where T : class, IMinecraftStream
    {
        if (head != value)
        {
            var previousStreamBase = head;
            var currentStreamBase = previousStreamBase;

            while (currentStreamBase is not null)
            {
                if (currentStreamBase is not IMinecraftStream currentStream || previousStreamBase is not IMinecraftStream previousStream)
                    break;

                if (currentStream == value)
                    previousStream.BaseStream = currentStream.BaseStream;

                previousStreamBase = currentStream;
                currentStreamBase = currentStream.BaseStream;
            }
        }
        else if (value.BaseStream is not null)
        {
            head = value.BaseStream;
        }
        else
        {
            throw new InvalidOperationException($"Cannot remove {value} stream with unset BaseStream");
        }
    }

    public T Get<T>() where T : class, IMinecraftStreamBase
    {
        if (Search<T>(out var stream))
            return stream;

        throw new InvalidOperationException($"{typeof(T)} not found in channel");
    }

    public bool Search<T>([MaybeNullWhen(false)] out T result) where T : class, IMinecraftStreamBase
    {
        return Get(head, out result);
    }

    public void PrependBuffer(Memory<byte> memory)
    {
        var stream = Get<IMinecraftNetworkStream>();
        stream.PrependBuffer(memory);
    }

    public async ValueTask<IMinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        if (_pause is not null)
            await _pause.Task;

        return head switch
        {
            IMinecraftPacketMessageStream stream => await stream.ReadPacketAsync(cancellationToken),
            IMinecraftCompleteMessageStream stream => await stream.ReadMessageAsync(cancellationToken),
            IMinecraftBufferedMessageStream stream => await stream.ReadAsMessageAsync(cancellationToken: cancellationToken),
            _ => throw new InvalidOperationException($"{head.GetType()} cannot be used to read messages")
        };
    }

    public async ValueTask WriteMessageAsync(IMinecraftMessage message, CancellationToken cancellationToken = default)
    {
        switch (head)
        {
            case IMinecraftPacketMessageStream stream:
                await stream.WritePacketAsync(message as IMinecraftPacket ?? throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(IMinecraftPacket)}."), cancellationToken);
                break;
            case IMinecraftCompleteMessageStream stream:
                await stream.WriteMessageAsync(message as CompleteBinaryMessage ?? throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(CompleteBinaryMessage)}."), cancellationToken);
                break;
            case IMinecraftBufferedMessageStream stream:
                await stream.WriteAsMessageAsync(message as BufferedBinaryMessage ?? throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(CompleteBinaryMessage)}."), cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"{head.GetType()} cannot be used to write messages");
        }
    }

    public void Pause()
    {
        if (_pause is { Task.IsCompleted: false })
            throw new InvalidOperationException($"{nameof(IMinecraftChannel)} is already paused");

        _pause = new TaskCompletionSource();
    }

    public void Resume()
    {
        if (_pause is null or { Task.IsCompleted: true })
            throw new InvalidOperationException($"{nameof(IMinecraftChannel)} is not paused");

        _pause.SetResult();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await head.FlushAsync(cancellationToken);
    }

    public void Close()
    {
        head.Close();
    }

    public void Dispose()
    {
        head.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await head.DisposeAsync();
    }

    public void Flush()
    {
        head.Flush();
    }

    private bool Get<T>(IMinecraftStreamBase? baseStream, [MaybeNullWhen(false)] out T result) where T : class, IMinecraftStreamBase
    {
        var current = baseStream ?? head;

        while (true)
            switch (current)
            {
                case T found:
                    result = found;
                    return true;
                case IMinecraftStream stream:
                    current = stream.BaseStream;
                    break;
                default:
                    result = null;
                    return false;
            }
    }
}