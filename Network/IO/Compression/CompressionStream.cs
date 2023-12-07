namespace MinecraftProxy.Network.IO.Compression;

public class CompressionStream(Stream baseStream, int threshold) : Stream
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

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return baseStream.ReadAsync(buffer, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return baseStream.WriteAsync(buffer, cancellationToken);
    }
}