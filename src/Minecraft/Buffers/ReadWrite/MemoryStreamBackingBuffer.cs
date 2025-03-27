using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;

namespace Void.Minecraft.Buffers.ReadWrite;

internal ref struct MemoryStreamBackingBuffer(MemoryStream memoryStream)
{
    public readonly long Position => memoryStream.Position;
    public long Length { get; } = memoryStream.Length;

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
        // TODO may be unsafe stackalloc?
        var block = ArrayPool<byte>.Shared.Rent(2);
        var span = block.AsSpan(0, 2);

        try
        {
            BinaryPrimitives.WriteUInt16BigEndian(span, value);
            memoryStream.Write(span);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(block);
        }
    }

    public readonly int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public readonly void WriteInt(int value)
    {
        // TODO may be unsafe stackalloc?
        var block = ArrayPool<byte>.Shared.Rent(4);
        var span = block.AsSpan(0, 4);

        try
        {
            BinaryPrimitives.WriteInt32BigEndian(span, value);
            memoryStream.Write(span);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(block);
        }
    }

    public readonly float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(Read(4));
    }

    public readonly void WriteFloat(float value)
    {
        // TODO may be unsafe stackalloc?
        var block = ArrayPool<byte>.Shared.Rent(4);
        var span = block.AsSpan(0, 4);

        try
        {
            BinaryPrimitives.WriteSingleBigEndian(span, value);
            memoryStream.Write(span);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(block);
        }
    }

    public readonly long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public readonly void WriteLong(long value)
    {
        // TODO may be unsafe stackalloc?
        var block = ArrayPool<byte>.Shared.Rent(8);
        var span = block.AsSpan(0, 8);

        try
        {
            BinaryPrimitives.WriteInt64BigEndian(span, value);
            memoryStream.Write(span);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(block);
        }
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