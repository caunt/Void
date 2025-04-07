using ICSharpCode.SharpZipLib.Zip.Compression;
using Microsoft.IO;
using System.Buffers;
using Void.Common.Network.Streams;
using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Network.Streams.Compression;
using Void.Proxy.Api.Network.Streams.Manual;
using Void.Proxy.Api.Network.Streams.Manual.Binary;
using Void.Proxy.Api.Network.Streams.Recyclable;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.Streams.Extensions;

namespace Void.Proxy.Plugins.Common.Network.Streams.Compression;

public class SharpZipLibCompressionMessageStream : MinecraftRecyclableStream, ICompleteMessageStream, IZlibCompressionStream
{
    private const int BufferSize = 1024;

    private readonly Deflater _compressor = new(Deflater.DEFAULT_COMPRESSION);
    private readonly Inflater _decompressor = new();

    public int CompressionThreshold { get; set; } = 256;
    public INetworkStreamBase? BaseStream { get; set; }
    public bool CanRead => BaseStream?.CanRead ?? false;
    public bool CanWrite => BaseStream?.CanWrite ?? false;
    public bool IsAlive => BaseStream?.IsAlive ?? false;

    public ICompleteBinaryMessage ReadMessage()
    {
        return BaseStream switch
        {
            IManualStream manualStream => ReadManual(manualStream),
            // IMinecraftBufferedStream bufferedStream => ReadBufferPacket(bufferedStream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<ICompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IManualStream manualStream => await ReadManualAsync(manualStream, cancellationToken),
            // IMinecraftBufferedStream bufferedStream => await ReadBufferPacketAsync(bufferedStream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public void WriteMessage(ICompleteBinaryMessage message)
    {
        switch (BaseStream)
        {
            case IManualStream manualStream:
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
            case IManualStream manualStream:
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

    private CompleteBinaryMessage ReadManual(IManualStream manualStream)
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

    private async ValueTask<ICompleteBinaryMessage> ReadManualAsync(IManualStream manualStream, CancellationToken cancellationToken = default)
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

    private void WriteManual(IManualStream manualStream, ICompleteBinaryMessage message)
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

    private async ValueTask WriteManualAsync(IManualStream manualStream, ICompleteBinaryMessage message, CancellationToken cancellationToken = default)
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

    private RecyclableMemoryStream Compress(RecyclableMemoryStream data)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

        try
        {
            foreach (var chunk in data.GetReadOnlySequence())
            {
                var chunkLength = chunk.Length;
                var chunkArray = ArrayPool<byte>.Shared.Rent(chunkLength);

                try
                {
                    chunk.CopyTo(chunkArray);
                    _compressor.SetInput(chunkArray, 0, chunkLength);

                    while (!_compressor.IsNeedingInput && _compressor.Deflate(buffer) is var compressedLength and not 0)
                        stream.Write(buffer, 0, compressedLength);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(chunkArray);
                }
            }

            _compressor.Finish();

            while (!_compressor.IsFinished && _compressor.Deflate(buffer) is var compressedLength and not 0)
                stream.Write(buffer, 0, compressedLength);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            _compressor.Reset();
        }

        return stream;
    }

    private RecyclableMemoryStream Decompress(RecyclableMemoryStream data)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

        try
        {
            foreach (var chunk in data.GetReadOnlySequence())
            {
                var chunkLength = chunk.Length;
                var chunkArray = ArrayPool<byte>.Shared.Rent(chunkLength);

                try
                {
                    chunk.CopyTo(chunkArray);
                    _decompressor.SetInput(chunkArray, 0, chunkLength);

                    while (!_decompressor.IsNeedingInput && _decompressor.Inflate(buffer) is var decompressedLength and not 0)
                        stream.Write(buffer, 0, decompressedLength);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(chunkArray);
                }
            }

            while (!_decompressor.IsFinished && _decompressor.Inflate(buffer) is var decompressedLength and not 0)
                stream.Write(buffer, 0, decompressedLength);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            _decompressor.Reset();
        }

        return stream;
    }
}
