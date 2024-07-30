using Ionic.Zlib;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Memory;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams.Extensions;

namespace Void.Proxy.API.Network.IO.Streams.Compression;

public class ZlibCompressionMessageStream : IMinecraftCompleteMessageStream
{
    public int CompressionThreshold { get; set; } = 256;
    public IMinecraftStreamBase? BaseStream { get; set; }

    public CompleteBinaryMessage ReadMessage()
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => ReadNetworkMessage(networkStream),
            // IMinecraftBufferedStream bufferedStream => ReadBufferPacket(bufferedStream),
            _ => throw new NotImplementedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<CompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => await ReadNetworkMessageAsync(networkStream, cancellationToken),
            // IMinecraftBufferedStream bufferedStream => await ReadBufferPacketAsync(bufferedStream),
            _ => throw new NotImplementedException(BaseStream?.GetType().FullName)
        };
    }

    public void WriteMessage(CompleteBinaryMessage message)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                WriteNetworkMessage(networkStream, message);
                break;
            default:
                throw new NotImplementedException(BaseStream?.GetType().FullName);
        }
    }

    public async ValueTask WriteMessageAsync(CompleteBinaryMessage message, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                await WriteNetworkMessageAsync(networkStream, message, cancellationToken);
                break;
            default:
                throw new NotImplementedException(BaseStream?.GetType().FullName);
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
            var holder = MemoryHolder.RentExact(packetLength - 1);
            stream.ReadExactly(holder.Slice.Span);
            return new CompleteBinaryMessage(holder);
        }
        else
        {
            using var holder = MemoryHolder.RentExact(packetLength - MinecraftBuffer.GetVarIntSize(dataLength));
            stream.ReadExactly(holder.Slice.Span);

            // TODO replace with something Span-compatible
            var dataHolder = MemoryHolder.From(ZlibStream.UncompressBuffer(holder.Slice.Span.ToArray()));

            if (dataHolder.Slice.Length != dataLength)
                throw new InvalidOperationException($"Received dataLength is {dataLength}, but uncompressed data length is {dataHolder.Slice.Length}");

            return new CompleteBinaryMessage(dataHolder);
        }
    }

    private static async ValueTask<CompleteBinaryMessage> ReadNetworkMessageAsync(IMinecraftNetworkStream stream, CancellationToken cancellationToken = default)
    {
        var packetLength = await stream.ReadVarIntAsync(cancellationToken);
        var dataLength = await stream.ReadVarIntAsync(cancellationToken);

        if (dataLength is 0)
        {
            var holder = MemoryHolder.RentExact(packetLength - 1);
            await stream.ReadExactlyAsync(holder.Slice, cancellationToken);
            return new CompleteBinaryMessage(holder);
        }
        else
        {
            using var holder = MemoryHolder.RentExact(packetLength - MinecraftBuffer.GetVarIntSize(dataLength));
            await stream.ReadExactlyAsync(holder.Slice, cancellationToken);

            // TODO replace with something Span-compatible
            var dataHolder = MemoryHolder.From(ZlibStream.UncompressBuffer(holder.Slice.Span.ToArray()));

            if (dataHolder.Slice.Length != dataLength)
                throw new InvalidOperationException($"Received dataLength is {dataLength}, but uncompressed data length is {dataHolder.Slice.Length}");

            return new CompleteBinaryMessage(dataHolder);
        }
    }

    private void WriteNetworkMessage(IMinecraftNetworkStream stream, CompleteBinaryMessage message)
    {
        var dataLength = message.Holder.Slice.Length < CompressionThreshold ? 0 : message.Holder.Slice.Length;
        var data = dataLength switch
        {
            > 0 => ZlibStream.CompressBuffer(message.Holder.Slice.Span.ToArray()),
            0 => message.Holder.Slice,
            _ => throw new InvalidOperationException($"{nameof(dataLength)} cannot be less than 0")
        };

        var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + data.Length;

        stream.WriteVarInt(packetLength);
        stream.WriteVarInt(dataLength);
        stream.Write(data.Span);
    }

    private async ValueTask WriteNetworkMessageAsync(IMinecraftNetworkStream stream, CompleteBinaryMessage message, CancellationToken cancellationToken = default)
    {
        var dataLength = message.Holder.Slice.Length < CompressionThreshold ? 0 : message.Holder.Slice.Length;
        var data = dataLength switch
        {
            > 0 => ZlibStream.CompressBuffer(message.Holder.Slice.Span.ToArray()),
            0 => message.Holder.Slice,
            _ => throw new InvalidOperationException($"{nameof(dataLength)} cannot be less than 0")
        };

        var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + data.Length;

        await stream.WriteVarIntAsync(packetLength, cancellationToken);
        await stream.WriteVarIntAsync(dataLength, cancellationToken);
        await stream.WriteAsync(data, cancellationToken);
    }
}