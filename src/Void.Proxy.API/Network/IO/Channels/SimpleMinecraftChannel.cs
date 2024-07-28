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
        Add(new T());
    }

    public void PrependBuffer(Memory<byte> memory)
    {
        var stream = GetNetworkStream(head);
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

    public void Add<T>(T stream) where T : IMinecraftStream
    {
        stream.BaseStream = head;
        head = stream;
    }

    private static IMinecraftNetworkStream GetNetworkStream(IMinecraftStreamBase? stream)
    {
        while (true)
        {
            switch (stream)
            {
                case IMinecraftNetworkStream minecraftNetworkStream:
                    return minecraftNetworkStream;
                case null:
                    throw new Exception($"{nameof(IMinecraftNetworkStream)} not found");
            }

            if (stream is not IMinecraftStream minecraftStream)
                throw new InvalidDataException($"Unexpected stream type \"{stream.GetType()}\" found");

            stream = minecraftStream.BaseStream;
        }
    }
}