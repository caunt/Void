using System;
using System.Buffers.Binary;
using System.IO;

namespace Void.Minecraft.Buffers.ReadWrite;

internal ref struct MemoryStreamBackingBuffer(MemoryStream memoryStream)
{
    public readonly long Position => memoryStream.Position;
    public readonly long Length => memoryStream.Length;

    public readonly byte ReadUnsignedByte()
    {
        var result = memoryStream.ReadByte();

        if (result < 0)
            throw new InternalBufferOverflowException();

        return (byte)result;
    }

    public readonly void WriteUnsignedByte(byte value)
    {
        memoryStream.WriteByte(value);
    }

    public readonly ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(Read(2));
    }

    public readonly void WriteUnsignedShort(ushort value)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        memoryStream.Write(buffer);
    }

    public readonly short ReadShort()
    {
        return BinaryPrimitives.ReadInt16BigEndian(Read(2));
    }

    public readonly void WriteShort(short value)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        memoryStream.Write(buffer);
    }

    public readonly int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public readonly void WriteInt(int value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        memoryStream.Write(buffer);
    }

    public readonly float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(Read(4));
    }

    public readonly void WriteFloat(float value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteSingleBigEndian(buffer, value);
        memoryStream.Write(buffer);
    }


    public readonly double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleBigEndian(Read(8));
    }

    public readonly void WriteDouble(double value)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
        memoryStream.Write(buffer);
    }

    public readonly long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public readonly void WriteLong(long value)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(buffer, value);
        memoryStream.Write(buffer);
    }

    public readonly void Seek(long offset, SeekOrigin origin)
    {
        memoryStream.Seek(offset, origin);
    }

    public readonly void Reset()
    {
        memoryStream.Position = 0;
    }

    public readonly ReadOnlySpan<byte> Read(long length)
    {
        // TODO is there way to get ReadOnlySequence from MemoryStream? do not allocate please
        var block = new byte[length];
        memoryStream.ReadExactly(block);
        return block;
    }

    public readonly void Write(ReadOnlySpan<byte> data)
    {
        memoryStream.Write(data);
    }

    public readonly void Write(Stream stream)
    {
        stream.CopyTo(memoryStream);
    }
}
