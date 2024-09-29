using System.Buffers.Binary;

namespace Void.Proxy.API.Network.IO.Buffers.ReadWrite;

internal ref struct SpanBackingBuffer(Span<byte> span)
{
    private readonly Span<byte> _block = span;

    public int Position = 0;
    public int Length { get; } = span.Length;

    public byte ReadUnsignedByte()
    {
        return _block[Position++];
    }

    public void WriteUnsignedByte(byte value)
    {
        _block[Position++] = value;
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(Slice(2));
    }

    public void WriteUnsignedShort(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(Slice(2), value);
    }

    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public void WriteInt(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(Slice(4), value);
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public void WriteLong(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(Slice(8), value);
    }

    public void Seek(int offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;
            case SeekOrigin.Current:
                Position += offset;
                break;
            case SeekOrigin.End:
                Position = _block.Length - offset;
                break;
            default:
                throw new InvalidOperationException();
        }

        if (Position < 0 || Position > _block.Length)
            throw new IndexOutOfRangeException($"Position {Position}, Length {_block.Length}");
    }

    public void Reset()
    {
        Position = 0;
    }

    public Span<byte> Slice(int length)
    {
        if (_block.Length < Position + length)
            throw new InternalBufferOverflowException($"Buffer length with max {_block.Length} at position {Position} attempted to slice {length} bytes");

        var span = _block.Slice(Position, length);
        Position += length;
        return span;
    }

    public ReadOnlySpan<byte> Read(int length)
    {
        return Slice(length);
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        if (_block.Length < Position + data.Length)
            throw new InternalBufferOverflowException($"Buffer length with max {_block.Length} at position {Position} attempted to write {data.Length} bytes");

        var span = _block.Slice(Position, data.Length);
        Position += data.Length;
        data.CopyTo(span);
    }
}