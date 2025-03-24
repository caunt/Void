using System.Buffers.Binary;

namespace Void.Proxy.Api.Network.IO.Buffers.ReadOnly;

internal ref struct ReadOnlySpanBackingBuffer(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> _block = span;

    public int Position = 0;
    public int Length { get; } = span.Length;

    public byte ReadUnsignedByte()
    {
        return _block[Position++];
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(Slice(2));
    }

    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
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

    public ReadOnlySpan<byte> Slice(int length)
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
}