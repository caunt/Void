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
        head = new T { BaseStream = head };
    }

    public void PrependBuffer(Memory<byte> memory)
    {
        var stream = GetNetworkStream(head);
        stream.PrependBuffer(memory);
    }

    public async ValueTask<IMinecraftMessage> ReadMessageAsync()
    {
        return Head switch
        {
            IMinecraftPacketMessageStream stream => await stream.ReadPacketAsync(),
            IMinecraftCompleteMessageStream stream => await stream.ReadMessageAsync(),
            IMinecraftBufferedMessageStream stream => await stream.ReadAsMessageAsync(2048),
            _ => throw new InvalidOperationException($"{head.GetType()} cannot be used to read messages")
        };
    }

    public async ValueTask WriteMessageAsync(IMinecraftMessage message)
    {
        switch (head)
        {
            case IMinecraftPacketMessageStream stream:
                await stream.WritePacketAsync(message as IMinecraftPacket ?? throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(IMinecraftPacket)}."));
                break;
            case IMinecraftCompleteMessageStream stream:
                await stream.WriteMessageAsync(message is CompleteBinaryMessage completeBinaryMessage ? completeBinaryMessage : throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(CompleteBinaryMessage)}."));
                break;
            case IMinecraftBufferedMessageStream stream:
                await stream.WriteAsMessageAsync(message is BufferedBinaryMessage bufferedBinaryMessage ? bufferedBinaryMessage : throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(CompleteBinaryMessage)}."));
                break;
            default:
                throw new InvalidOperationException($"{head.GetType()} cannot be used to write messages");
        }
    }

    public void Flush()
    {
        Head.Flush();
    }

    public async ValueTask FlushAsync()
    {
        await Head.FlushAsync();
    }

    public void Close()
    {
        Head.Close();
    }

    public void Dispose()
    {
        Head.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Head.DisposeAsync();
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