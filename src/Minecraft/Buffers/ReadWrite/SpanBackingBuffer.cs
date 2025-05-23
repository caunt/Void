﻿using System;
using System.Buffers.Binary;
using System.IO;

namespace Void.Minecraft.Buffers.ReadWrite;

internal ref struct SpanBackingBuffer(Span<byte> span)
{
    private readonly Span<byte> _block = span;

    public long Position = 0;
    public long Length { get; } = span.Length;

    public byte ReadUnsignedByte()
    {
        return _block[(int)Position++];
    }

    public void WriteUnsignedByte(byte value)
    {
        _block[(int)Position++] = value;
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(Slice(2));
    }

    public void WriteUnsignedShort(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(Slice(2), value);
    }

    public short ReadShort()
    {
        return BinaryPrimitives.ReadInt16BigEndian(Slice(2));
    }

    public void WriteShort(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(Slice(2), value);
    }

    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public void WriteInt(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(Slice(4), value);
    }

    public float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(Read(4));
    }

    public void WriteFloat(float value)
    {
        BinaryPrimitives.WriteSingleBigEndian(Slice(4), value);
    }

    public double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleBigEndian(Read(8));
    }

    public void WriteDouble(double value)
    {
        BinaryPrimitives.WriteDoubleBigEndian(Slice(8), value);
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public void WriteLong(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(Slice(8), value);
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

    public Span<byte> Slice(long length)
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

    public void Write(ReadOnlySpan<byte> data)
    {
        if (_block.Length < Position + data.Length)
            throw new InternalBufferOverflowException($"Buffer length with max {_block.Length} at position {Position} attempted to write {data.Length} bytes");

        var span = _block.Slice((int)Position, data.Length);
        Position += data.Length;
        data.CopyTo(span);
    }

    public void Write(Stream stream)
    {
        var length = (int)stream.Length;
        if (_block.Length < Position + length)
            throw new InternalBufferOverflowException($"Buffer length with max {_block.Length} at position {Position} attempted to write {stream.Length} bytes");

        var span = _block.Slice((int)Position, length);
        Position += length;

        stream.ReadExactly(span);
    }
}
