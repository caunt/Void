namespace MinecraftProxy.Network.IO.Common;

internal class ReadWriteStream(Stream readStream, Stream writeStream) : Stream
{
    public override bool CanRead => readStream.CanRead;

    public override bool CanWrite => writeStream.CanWrite;

    public override bool CanSeek => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return readStream.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        writeStream.Write(buffer, offset, count);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return readStream.ReadAsync(buffer, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return writeStream.WriteAsync(buffer, cancellationToken);
    }

    public override void Flush()
    {
        readStream.Flush();
        writeStream.Flush();
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        await writeStream.FlushAsync(cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            readStream.Dispose();
            writeStream.Dispose();
        }

        base.Dispose(disposing);
    }
}