using MinecraftProxy.Network.IO.Compression;
using System.Buffers;
using System.IO;

namespace MinecraftProxy.Network.IO.Common;

public class PacketStream(Stream baseStream) : Stream
{
    public override bool CanRead => baseStream.CanRead;

    public override bool CanSeek => baseStream.CanSeek;

    public override bool CanWrite => baseStream.CanWrite;

    public override long Length => baseStream.Length;

    public override long Position { get => baseStream.Position; set => baseStream.Position = value; }

    public override void Flush() => baseStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => baseStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);

    public override void SetLength(long value) => baseStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => baseStream.Write(buffer, offset, count);

    // not used, prefer ReadPacketAsync
    public override async ValueTask<int> ReadAsync(Memory<byte> output, CancellationToken cancellationToken = default)
    {
        var length = await baseStream.ReadVarIntAsync(cancellationToken);
        await baseStream.ReadExactlyAsync(output[..length], cancellationToken);
        return length;
    }

    public async ValueTask<MinecraftMessage> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        if (baseStream is DeprecatedCompressionStream compressionStream)
            return await compressionStream.ReadPacketAsync(cancellationToken);

        var length = await baseStream.ReadVarIntAsync(cancellationToken);
        return await baseStream.ReadMessageAsync(length, cancellationToken);
    }

    // not used, prefer WritePacketAsync
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> output, CancellationToken cancellationToken = default)
    {
        await baseStream.WriteVarIntAsync(output.Length, cancellationToken);
        await baseStream.WriteAsync(output, cancellationToken);
    }

    public async ValueTask WritePacketAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        if (baseStream is DeprecatedCompressionStream compressionStream)
        {
            await compressionStream.WritePacketAsync(message, cancellationToken);
            return;
        }

        var length = message.Memory.Length;

        await baseStream.WriteVarIntAsync(length, cancellationToken);
        await baseStream.WriteAsync(message.Memory, cancellationToken);
    }
}