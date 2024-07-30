using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams;

namespace Void.Proxy.API.Network.IO.Channels;

public class SimpleMinecraftChannel(IMinecraftStreamBase head) : IMinecraftChannel
{
    public bool CanRead => true;
    public bool CanWrite => true;

    public IMinecraftStreamBase Head => head;
    public bool IsConfigured => head is IMinecraftStream;

    public void Add<T>() where T : IMinecraftStream, new()
    {
        var stream = new T();
        stream.BaseStream = head;
        head = stream;
    }

    public T Get<T>() where T : IMinecraftStreamBase
    {
        return Get<T>(head);
    }

    public void AddBefore<TBefore, TValue>() where TBefore : IMinecraftStream where TValue : IMinecraftStream, new()
    {
        var before = Get<TBefore>();
        var beforeBaseStream = before.BaseStream;
        var addedBaseStream = new TValue { BaseStream = beforeBaseStream };
        before.BaseStream = addedBaseStream;
    }

    public void PrependBuffer(Memory<byte> memory)
    {
        var stream = Get<IMinecraftNetworkStream>();
        stream.PrependBuffer(memory);
    }

    public async ValueTask<IMinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
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
                await stream.WriteMessageAsync(message is CompleteBinaryMessage completeBinaryMessage ? completeBinaryMessage : throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(CompleteBinaryMessage)}."), cancellationToken);
                break;
            case IMinecraftBufferedMessageStream stream:
                await stream.WriteAsMessageAsync(message is BufferedBinaryMessage bufferedBinaryMessage ? bufferedBinaryMessage : throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(CompleteBinaryMessage)}."), cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"{head.GetType()} cannot be used to write messages");
        }
    }

    public void Flush()
    {
        head.Flush();
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

    private T Get<T>(IMinecraftStreamBase? baseStream) where T : IMinecraftStreamBase
    {
        var current = baseStream ?? head;
        while (true)
            switch (current)
            {
                case T found:
                    return found;
                case IMinecraftStream stream:
                    current = stream.BaseStream;
                    break;
                default:
                    throw new InvalidOperationException($"{typeof(T)} not found in channel");
            }
    }
}