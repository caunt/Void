using Ionic.Zlib;
using System.Buffers;

namespace MinecraftProxy.Network.IO.Compression;

public class DeprecatedCompressionStream(Stream baseStream, int threshold) : Stream
{
    public override bool CanRead => baseStream.CanRead;

    public override bool CanSeek => baseStream.CanSeek;

    public override bool CanWrite => baseStream.CanWrite;

    public override long Length => baseStream.Length;

    public override long Position { get => baseStream.Position; set => baseStream.Position = value; }

    public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);

    public override void SetLength(long value) => baseStream.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override void Flush() => baseStream.Flush();

    public override async Task FlushAsync(CancellationToken cancellationToken) => await baseStream.FlushAsync(cancellationToken);

    public override ValueTask<int> ReadAsync(Memory<byte> output, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> output, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async ValueTask<MinecraftMessage> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        var packetLength = await baseStream.ReadVarIntAsync(cancellationToken);
        var dataLength = await baseStream.ReadVarIntAsync(cancellationToken);

        if (dataLength is 0)
            return await baseStream.ReadMessageAsync(packetLength - MinecraftBuffer.GetVarIntSize(dataLength), cancellationToken);

        var rent = ArrayPool<byte>.Shared.Rent(packetLength);
        var buffer = rent[..(packetLength - MinecraftBuffer.GetVarIntSize(dataLength))];

        var memoryOwner = MemoryPool<byte>.Shared.Rent(dataLength);
        var memory = memoryOwner.Memory[..dataLength];

        try
        {
            await baseStream.ReadExactlyAsync(buffer, cancellationToken);
            ZlibStream.UncompressBuffer(buffer).CopyTo(memory);
            var packetId = new MinecraftBuffer(memory).ReadVarInt();
            return new(packetId, memory[MinecraftBuffer.GetVarIntSize(packetId)..], memoryOwner);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rent);
        }
    }

    public async ValueTask WritePacketAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        var dataLength = message.Length + MinecraftBuffer.GetVarIntSize(message.PacketId);

        if (dataLength < threshold)
        {
            var length = dataLength + MinecraftBuffer.GetVarIntSize(0);

            await baseStream.WriteVarIntAsync(length, cancellationToken);
            await baseStream.WriteVarIntAsync(0, cancellationToken);
            await baseStream.WriteVarIntAsync(message.PacketId, cancellationToken);
            await baseStream.WriteAsync(message.Memory, cancellationToken);

            return;
        }

        var compressedData = ZlibStream.CompressBuffer([.. MinecraftBuffer.GetVarInt(message.PacketId), .. message.Memory.Span]);

        await baseStream.WriteVarIntAsync(compressedData.Length + MinecraftBuffer.GetVarIntSize(dataLength), cancellationToken);
        await baseStream.WriteVarIntAsync(dataLength, cancellationToken);
        await baseStream.WriteAsync(compressedData, cancellationToken);
    }
}