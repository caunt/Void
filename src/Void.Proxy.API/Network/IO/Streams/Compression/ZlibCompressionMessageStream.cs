using System.Buffers;
using Ionic.Zlib;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams.Extensions;

namespace Void.Proxy.API.Network.IO.Streams.Compression;

public class ZlibCompressionMessageStream : IMinecraftCompleteMessageStream
{
    public IMinecraftStreamBase? BaseStream { get; set; }

    public CompleteBinaryMessage ReadMessage()
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => ReadNetworkMessage(networkStream),
            // IMinecraftBufferedStream bufferedStream => ReadBufferPacket(bufferedStream),
            _ => throw new NotImplementedException()
        };
    }

    public async ValueTask<CompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => await ReadNetworkMessageAsync(networkStream, cancellationToken),
            // IMinecraftBufferedStream bufferedStream => await ReadBufferPacketAsync(bufferedStream),
            _ => throw new NotImplementedException()
        };
    }

    public void WriteMessage(CompleteBinaryMessage message)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                // WriteNetworkMessage(networkStream);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public async ValueTask WriteMessageAsync(CompleteBinaryMessage message, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                // await WriteNetworkMessageAsync(networkStream);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void Dispose()
    {
        BaseStream?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (BaseStream != null)
            await BaseStream.DisposeAsync();
    }

    public void Flush()
    {
        BaseStream?.Flush();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        if (BaseStream != null)
            await BaseStream.FlushAsync(cancellationToken);
    }

    public void Close()
    {
        BaseStream?.Close();
    }

    private static CompleteBinaryMessage ReadNetworkMessage(IMinecraftNetworkStream stream)
    {
        var packetLength = stream.ReadVarInt();
        var dataLength = stream.ReadVarInt();

        if (dataLength is 0)
        {
            var length = packetLength - MinecraftBuffer.GetVarIntSize(dataLength);
            var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
            var memory = memoryOwner.Memory[..length];

            stream.ReadExactly(memory.Span);
            return new CompleteBinaryMessage(memory, memoryOwner);
        }
        else
        {
            var rent = ArrayPool<byte>.Shared.Rent(packetLength);
            var buffer = rent[..(packetLength - MinecraftBuffer.GetVarIntSize(dataLength))];

            var memoryOwner = MemoryPool<byte>.Shared.Rent(dataLength);
            var memory = memoryOwner.Memory[..dataLength];

            try
            {
                stream.ReadExactly(buffer);
                ZlibStream.UncompressBuffer(buffer).CopyTo(memory);
                return new CompleteBinaryMessage(memory, memoryOwner);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }
    }

    private static async ValueTask<CompleteBinaryMessage> ReadNetworkMessageAsync(IMinecraftNetworkStream stream, CancellationToken cancellationToken = default)
    {
        var packetLength = await stream.ReadVarIntAsync(cancellationToken);
        var dataLength = await stream.ReadVarIntAsync(cancellationToken);

        if (dataLength is 0)
        {
            var length = packetLength - MinecraftBuffer.GetVarIntSize(dataLength);
            var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
            var memory = memoryOwner.Memory[..length];

            await stream.ReadExactlyAsync(memory, cancellationToken);
            return new CompleteBinaryMessage(memory, memoryOwner);
        }
        else
        {
            var rent = ArrayPool<byte>.Shared.Rent(packetLength);
            var buffer = rent[..(packetLength - MinecraftBuffer.GetVarIntSize(dataLength))];

            var memoryOwner = MemoryPool<byte>.Shared.Rent(dataLength);
            var memory = memoryOwner.Memory[..dataLength];

            try
            {
                await stream.ReadExactlyAsync(buffer, cancellationToken);
                ZlibStream.UncompressBuffer(buffer).CopyTo(memory);
                return new CompleteBinaryMessage(memory, memoryOwner);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }
    }
}