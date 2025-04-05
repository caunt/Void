using System;
using System.Buffers.Binary;
using System.IO;

namespace Void.Minecraft.Buffers.ReadOnly;

internal ref struct ReadOnlySpanBackingBuffer(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> _block = span;

    public long Position = 0;
    public long Length { get; } = span.Length;

    public byte ReadUnsignedByte()
    {
        return _block[(int)Position++];
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(Slice(2));
    }

    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(Read(4));
    }

    public double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleBigEndian(Read(8));
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public void Seek(long offset, SeekOrigin origin)
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

    public ReadOnlySpan<byte> Slice(long length)
    {
        if (_block.Length < Position + length)
            throw new InternalBufferOverflowException($"Buffer length with max {_block.Length} at position {Position} attempted to slice {length} bytes");

        var span = _block.Slice((int)Position, (int)length);
        Position += length;
        return span;
    }

    public ReadOnlySpan<byte> Read(long length)
    {
        return Slice(length);
    }
}