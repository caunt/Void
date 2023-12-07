using MinecraftProxy.Models;
using MinecraftProxy.Utils;
using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;
using System.Text;

namespace MinecraftProxy.Network.IO;

public ref struct MinecraftBuffer(Memory<byte> memory)
{
    public long Length => Span.Length;
    public bool HasData => Position < Length;
    public int Position { get; private set; }
    public Span<byte> Span { get; init; } = memory.Span;

    public MinecraftBuffer(int size) : this(MemoryPool<byte>.Shared.Rent(size).Memory) { } // is it safe? will GC dispose MemoryOwner too early?

    public MinecraftBuffer() : this(2048) { }

    public void Reset()
    {
        Position = 0;
    }

    public static int GetVarIntSize(int value)
    {
        return (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;
    }

    public static IEnumerable<byte> GetVarInt(int value)
    {
        var unsigned = (uint)value;

        do
        {
            var temp = (byte)(unsigned & 127);

            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            yield return temp;
        }
        while (unsigned != 0);
    }

    public int ReadVarInt()
    {
        int numRead = 0;
        int result = 0;
        byte read;
        do
        {
            read = ReadUnsignedByte();
            int value = read & 0b01111111;
            result |= value << 7 * numRead;

            numRead++;
            if (numRead > 5)
            {
                throw new InvalidOperationException("VarInt is too big");
            }
        } while ((read & 0b10000000) != 0);

        return result;
    }

    public void WriteVarInt(int value)
    {
        foreach (var temp in GetVarInt(value))
            WriteUnsignedByte(temp);
    }

    public byte ReadUnsignedByte()
    {
        return Span[Position++];
    }

    public void WriteUnsignedByte(byte value)
    {
        Span[Position++] = value;
    }

    public ushort ReadUnsignedShort()
    {
        var span = Span.Slice(Position, 2);
        Position += 2;
        return BinaryPrimitives.ReadUInt16BigEndian(span);
    }

    public void WriteUnsignedShort(ushort value)
    {
        var span = Span.Slice(Position, 2);
        Position += 2;
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
    }

    public string ReadString(int maxLength = 32767)
    {
        var length = ReadVarInt();
        var span = Span.Slice(Position, length);
        Position += length;

        var value = Encoding.UTF8.GetString(span);

        if (maxLength > 0 && value.Length > maxLength)
            throw new IndexOutOfRangeException($"string ({value.Length}) exceeded maximum length ({maxLength})");

        return value;
    }

    public void WriteString(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        WriteVarInt(length);

        var span = Span.Slice(Position, length);
        Position += length;

        Encoding.UTF8.GetBytes(value, span);
    }

    public bool ReadBoolean()
    {
        return ReadUnsignedByte() == 0x01;
    }

    public void WriteBoolean(bool value)
    {
        WriteUnsignedByte((byte)(value ? 0x01 : 0x00));
    }

    public Guid ReadGuid() => GuidHelper.FromLongs(ReadLong(), ReadLong());

    public void WriteGuid(Guid value)
    {
        if (value == Guid.Empty)
        {
            Write(new byte[16]);
        }
        else
        {
            Write(value.ToByteArray(true));
        }
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public void WriteLong(long value)
    {
        var span = Span.Slice(Position, 8);
        Position += 8;
        BinaryPrimitives.WriteInt64BigEndian(span, value);
    }

    public IdentifiedKey ReadIdentifiedKey() => new()
    {
        ExpiresAt = ReadLong(),
        PublicKey = Read(ReadVarInt()).ToArray(),
        KeySignature = Read(ReadVarInt()).ToArray()
    };

    public void WriteIdentifiedKey(IdentifiedKey identifiedKey)
    {
        WriteLong(identifiedKey.ExpiresAt);

        WriteVarInt(identifiedKey.PublicKey.Length);
        Write(identifiedKey.PublicKey);

        WriteVarInt(identifiedKey.KeySignature.Length);
        Write(identifiedKey.KeySignature);
    }

    public List<Property> ReadPropertyList(int count = -1)
    {
        if (count < 0)
            count = ReadVarInt();

        var list = new List<Property>();

        for (int i = 0; i < count; i++)
        {
            var name = ReadString();
            var value = ReadString();
            var isSigned = ReadBoolean();
            var signature = isSigned ? ReadString() : null;

            list.Add(new(name, value, isSigned, signature));
        }

        return list;
    }

    public void WritePropertyList(IEnumerable<Property> properties)
    {
        WriteVarInt(properties.Count());

        foreach (var property in properties)
        {
            WriteString(property.Name);
            WriteString(property.Value);
            WriteBoolean(property.IsSigned);

            if (property is { IsSigned: true, Signature: not null })
                WriteString(property.Signature);
        }
    }

    public Span<byte> Read(int length)
    {
        if (Length < Position + length)
            throw new InternalBufferOverflowException($"Buffer length with max {Length} at position {Position} attempted to read {length} bytes");

        var span = Span.Slice(Position, length);
        Position += length;
        return span;
    }

    public void Write(Span<byte> data)
    {
        if (Length < Position + data.Length)
            throw new InternalBufferOverflowException($"Buffer length with max {Length} at position {Position} attempted to write {data.Length} bytes");

        var span = Span.Slice(Position, data.Length);
        Position += data.Length;
        data.CopyTo(span);
    }
}