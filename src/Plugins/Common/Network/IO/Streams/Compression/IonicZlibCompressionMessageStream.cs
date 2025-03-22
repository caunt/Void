using Ionic.Zlib;
using Microsoft.IO;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages.Binary;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.IO.Streams.Compression;
using Void.Proxy.API.Network.IO.Streams.Extensions;
using Void.Proxy.API.Network.IO.Streams.Manual;
using Void.Proxy.API.Network.IO.Streams.Manual.Binary;
using Void.Proxy.API.Network.IO.Streams.Recyclable;
using Void.Proxy.Plugins.Common.Network.IO.Messages.Binary;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Compression;

public class IonicZlibCompressionMessageStream : MinecraftRecyclableStream, IMinecraftCompleteMessageStream, IZlibCompressionStream
{
    public int CompressionThreshold { get; set; } = 256;

    public IMinecraftStreamBase? BaseStream { get; set; }
    public bool CanRead => BaseStream?.CanRead ?? false;
    public bool CanWrite => BaseStream?.CanWrite ?? false;
    public bool IsAlive => BaseStream?.IsAlive ?? false;

    public ICompleteBinaryMessage ReadMessage()
    {
        return BaseStream switch
        {
            IMinecraftManualStream manualStream => ReadManual(manualStream),
            // IMinecraftBufferedStream bufferedStream => ReadBufferPacket(bufferedStream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<ICompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IMinecraftManualStream manualStream => await ReadManualAsync(manualStream, cancellationToken),
            // IMinecraftBufferedStream bufferedStream => await ReadBufferPacketAsync(bufferedStream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public void WriteMessage(ICompleteBinaryMessage message)
    {
        switch (BaseStream)
        {
            case IMinecraftManualStream manualStream:
                WriteManual(manualStream, message);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }
    }

    public async ValueTask WriteMessageAsync(ICompleteBinaryMessage message, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IMinecraftManualStream manualStream:
                await WriteManualAsync(manualStream, message, cancellationToken);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }
    }

    public void Dispose()
    {
        BaseStream?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (BaseStream != null)
            await BaseStream.DisposeAsync();

        GC.SuppressFinalize(this);
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

    private static CompleteBinaryMessage ReadManual(IMinecraftManualStream manualStream)
    {
        var packetLength = manualStream.ReadVarInt();
        var dataLength = manualStream.ReadVarInt();

        if (dataLength is 0)
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - 1;
            var buffer = stream.GetSpan(length);

            manualStream.ReadExactly(buffer[..length]);
            stream.Advance(length);

            return new CompleteBinaryMessage(stream);
        }
        else
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - MinecraftBuffer.GetVarIntSize(dataLength);
            var buffer = stream.GetSpan(length);

            manualStream.ReadExactly(buffer[..length]);
            stream.Advance(length);

            var dataHolder = Decompress(stream);

            if (dataHolder.Length != dataLength)
                throw new InvalidOperationException($"Received dataLength is {dataLength}, but uncompressed data length is {dataHolder.Length}");

            return new CompleteBinaryMessage(dataHolder);
        }
    }

    private static async ValueTask<ICompleteBinaryMessage> ReadManualAsync(IMinecraftManualStream manualStream, CancellationToken cancellationToken = default)
    {
        var packetLength = await manualStream.ReadVarIntAsync(cancellationToken);
        var dataLength = await manualStream.ReadVarIntAsync(cancellationToken);

        if (dataLength is 0)
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - 1;
            var buffer = stream.GetMemory(length);

            await manualStream.ReadExactlyAsync(buffer[..length], cancellationToken);
            stream.Advance(length);

            return new CompleteBinaryMessage(stream);
        }
        else
        {
            var stream = RecyclableMemoryStreamManager.GetStream();

            var length = packetLength - MinecraftBuffer.GetVarIntSize(dataLength);
            var buffer = stream.GetMemory(length);

            await manualStream.ReadExactlyAsync(buffer[..length], cancellationToken);
            stream.Advance(length);

            var dataHolder = Decompress(stream);

            if (dataHolder.Length != dataLength)
                throw new InvalidOperationException($"Received dataLength is {dataLength}, but uncompressed data length is {dataHolder.Length}");

            return new CompleteBinaryMessage(dataHolder);
        }
    }

    private void WriteManual(IMinecraftManualStream manualStream, ICompleteBinaryMessage message)
    {
        var dataLength = message.Stream.Length < CompressionThreshold ? 0 : (int)message.Stream.Length;

        if (dataLength > 0)
        {
            using var compressedStream = Compress(message.Stream);
            compressedStream.Position = 0;

            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)compressedStream.Length;
            manualStream.WriteVarInt(packetLength);
            manualStream.WriteVarInt(dataLength);

            foreach (var memory in compressedStream.GetReadOnlySequence())
                manualStream.Write(memory.Span);
        }
        else
        {
            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)message.Stream.Length;
            manualStream.WriteVarInt(packetLength);
            manualStream.WriteVarInt(dataLength);

            foreach (var memory in message.Stream.GetReadOnlySequence())
                manualStream.Write(memory.Span);
        }

        message.Dispose();
    }

    private async ValueTask WriteManualAsync(IMinecraftManualStream manualStream, ICompleteBinaryMessage message, CancellationToken cancellationToken = default)
    {
        var dataLength = message.Stream.Length < CompressionThreshold ? 0 : (int)message.Stream.Length;

        if (dataLength > 0)
        {
            await using var compressedStream = Compress(message.Stream);
            compressedStream.Position = 0;

            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)compressedStream.Length;

            await manualStream.WriteVarIntAsync(packetLength, cancellationToken);
            await manualStream.WriteVarIntAsync(dataLength, cancellationToken);

            foreach (var memory in compressedStream.GetReadOnlySequence())
                await manualStream.WriteAsync(memory, cancellationToken);
        }
        else
        {
            var packetLength = MinecraftBuffer.GetVarIntSize(dataLength) + (int)message.Stream.Length;

            await manualStream.WriteVarIntAsync(packetLength, cancellationToken);
            await manualStream.WriteVarIntAsync(dataLength, cancellationToken);

            foreach (var memory in message.Stream.GetReadOnlySequence())
                await manualStream.WriteAsync(memory, cancellationToken);
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