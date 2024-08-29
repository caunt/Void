using Ionic.Zlib;
using Microsoft.IO;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams.Extensions;

namespace Void.Proxy.API.Network.IO.Streams.Compression;

public class IonicZlibCompressionMessageStream : MinecraftRecyclableStream, IMinecraftCompleteMessageStream
{
    public int CompressionThreshold { get; set; } = 256;

    public IMinecraftStreamBase? BaseStream { get; set; }

    public CompleteBinaryMessage ReadMessage()
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => ReadNetworkMessage(networkStream),
            // IMinecraftBufferedStream bufferedStream => ReadBufferPacket(bufferedStream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<CompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => await ReadNetworkMessageAsync(networkStream, cancellationToken),
            // IMinecraftBufferedStream bufferedStream => await ReadBufferPacketAsync(bufferedStream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
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
                throw new NotSupportedException(BaseStream?.GetType().FullName);
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
                throw new NotSupportedException(BaseStream?.GetType().FullName);
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

    private static CompleteBinaryMessage ReadNetworkMessage(IMinecraftNetworkStream networkStream)
    {
        var packetLength = networkStream.ReadVarInt();
        var dataLength = networkStream.ReadVarInt();

        if (dataLength is 0)
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - 1;
            var buffer = stream.GetSpan(length);

            networkStream.ReadExactly(buffer[..length]);
            stream.Advance(length);

            return new CompleteBinaryMessage(stream);
        }
        else
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - MinecraftBuffer.GetVarIntSize(dataLength);
            var buffer = stream.GetSpan(length);

            networkStream.ReadExactly(buffer[..length]);
            stream.Advance(length);

            var dataHolder = Decompress(stream);

            if (dataHolder.Length != dataLength)
                throw new InvalidOperationException($"Received dataLength is {dataLength}, but uncompressed data length is {dataHolder.Length}");

            return new CompleteBinaryMessage(dataHolder);
        }
    }

    private static async ValueTask<CompleteBinaryMessage> ReadNetworkMessageAsync(IMinecraftNetworkStream networkStream, CancellationToken cancellationToken = default)
    {
        var packetLength = await networkStream.ReadVarIntAsync(cancellationToken);
        var dataLength = await networkStream.ReadVarIntAsync(cancellationToken);

        if (dataLength is 0)
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - 1;
            var buffer = stream.GetMemory(length);

            await networkStream.ReadExactlyAsync(buffer[..length], cancellationToken);
            stream.Advance(length);

            return new CompleteBinaryMessage(stream);
        }
        else
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - MinecraftBuffer.GetVarIntSize(dataLength);
            var buffer = stream.GetMemory(length);

            await networkStream.ReadExactlyAsync(buffer[..length], cancellationToken);
            stream.Advance(length);

            var dataHolder = Decompress(stream);

            if (dataHolder.Length != dataLength)
                throw new InvalidOperationException($"Received dataLength is {dataLength}, but uncompressed data length is {dataHolder.Length}");

            return new CompleteBinaryMessage(dataHolder);
        }
    }

    private void WriteNetworkMessage(IMinecraftNetworkStream stream, CompleteBinaryMessage message)
    {
        var dataLength = message.Stream.Length < CompressionThreshold ? 0 : (int)message.Stream.Length;

        if (dataLength > 0)
        {
            using var compressedStream = Compress(message.Stream);
            compressedStream.Position = 0;

            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)compressedStream.Length;
            stream.WriteVarInt(packetLength);
            stream.WriteVarInt(dataLength);

            foreach (var memory in compressedStream.GetReadOnlySequence())
                stream.Write(memory.Span);
        }
        else
        {
            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)message.Stream.Length;
            stream.WriteVarInt(packetLength);
            stream.WriteVarInt(dataLength);

            foreach (var memory in message.Stream.GetReadOnlySequence())
                stream.Write(memory.Span);
        }

        message.Dispose();
    }

    private async ValueTask WriteNetworkMessageAsync(IMinecraftNetworkStream stream, CompleteBinaryMessage message, CancellationToken cancellationToken = default)
    {
        var dataLength = message.Stream.Length < CompressionThreshold ? 0 : (int)message.Stream.Length;

        if (dataLength > 0)
        {
            await using var compressedStream = Compress(message.Stream);
            compressedStream.Position = 0;

            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)compressedStream.Length;

            await stream.WriteVarIntAsync(packetLength, cancellationToken);
            await stream.WriteVarIntAsync(dataLength, cancellationToken);

            foreach (var memory in compressedStream.GetReadOnlySequence())
                await stream.WriteAsync(memory, cancellationToken);
        }
        else
        {
            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)message.Stream.Length;

            await stream.WriteVarIntAsync(packetLength, cancellationToken);
            await stream.WriteVarIntAsync(dataLength, cancellationToken);

            foreach (var memory in message.Stream.GetReadOnlySequence())
                await stream.WriteAsync(memory, cancellationToken);
        }

        message.Dispose();
    }

    private static RecyclableMemoryStream Compress(RecyclableMemoryStream data)
    {
        // ArrayPool<byte> cannot rent exactly sized array, and ZlibStream does not accept Span<byte> to Slice them.
        var buffer = new byte[data.Length];

        data.Position = 0;
        data.ReadExactly(buffer);

        return RecyclableMemoryStreamManager.GetStream(ZlibStream.CompressBuffer(buffer));
    }

    private static RecyclableMemoryStream Decompress(RecyclableMemoryStream data)
    {
        // ArrayPool<byte> cannot rent exactly sized array, and ZlibStream does not accept Span<byte> to Slice them.
        var buffer = new byte[data.Length];

        data.Position = 0;
        data.ReadExactly(buffer);

        return RecyclableMemoryStreamManager.GetStream(ZlibStream.UncompressBuffer(buffer));
    }
}