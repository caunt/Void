using Void.Proxy.Network.IO.Compression;

namespace Void.Proxy.Network.IO.Common;

public class PacketStream(Stream baseStream) : Stream
{
    public override bool CanRead => baseStream.CanRead;

    public override bool CanSeek => baseStream.CanSeek;

    public override bool CanWrite => baseStream.CanWrite;

    public override long Length => baseStream.Length;

    public override long Position
    {
        get => baseStream.Position;
        set => baseStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return baseStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        baseStream.SetLength(value);
    }

    public override void Flush()
    {
        baseStream.Flush();
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        await baseStream.FlushAsync(cancellationToken);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        baseStream.Write(buffer, offset, count);
    }

    // not used, prefer ReadPacketAsync
    public override async ValueTask<int> ReadAsync(Memory<byte> output, CancellationToken cancellationToken = default)
    {
        // where can we get packetId?
        var length = await baseStream.ReadVarIntAsync(cancellationToken);
        await baseStream.ReadExactlyAsync(output[..length], cancellationToken);
        throw new NotSupportedException();
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
        // where can we get packetId?
        await baseStream.WriteVarIntAsync(output.Length, cancellationToken);
        await baseStream.WriteAsync(output, cancellationToken);
        throw new NotSupportedException();
    }

    public async ValueTask WritePacketAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        if (baseStream is DeprecatedCompressionStream compressionStream)
        {
            await compressionStream.WritePacketAsync(message, cancellationToken);
            return;
        }

        await baseStream.WriteVarIntAsync(message.Length + MinecraftBuffer.GetVarIntSize(message.PacketId), cancellationToken);
        await baseStream.WriteVarIntAsync(message.PacketId, cancellationToken);
        await baseStream.WriteAsync(message.Memory, cancellationToken);
    }
}