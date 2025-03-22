using System.Buffers;
using System.Buffers.Binary;

namespace Void.Proxy.API.Network.IO.Buffers.ReadWrite;

internal ref struct MemoryStreamBackingBuffer(MemoryStream memoryStream)
{
    public int Position => (int)memoryStream.Position;
    public int Length { get; } = (int)memoryStream.Length;

    public byte ReadUnsignedByte()
    {
        var result = memoryStream.ReadByte();

        if (result < 0)
            throw new InternalBufferOverflowException();

        return (byte)result;
    }

    public void WriteUnsignedByte(byte value)
    {
        memoryStream.WriteByte(value);
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(Read(2));
    }

    public void WriteUnsignedShort(ushort value)
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

    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public void WriteInt(int value)
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

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public void WriteLong(long value)
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

    public void Seek(int offset, SeekOrigin origin)
    {
        memoryStream.Seek(offset, origin);
    }

    public void Reset()
    {
        memoryStream.Position = 0;
    }

    public ReadOnlySpan<byte> Read(int length)
    {
        // TODO is there way to get ReadOnlySequence from MemoryStream? do not allocate please
        var block = new byte[length];
        memoryStream.ReadExactly(block);
        return block;
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        memoryStream.Write(data);
    }

    public void Write(Stream stream)
    {
        stream.CopyTo(memoryStream);
    }
}