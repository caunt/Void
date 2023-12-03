using MinecraftProxy.Models;
using MinecraftProxy.Utils;
using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MinecraftProxy.Network;

public class MinecraftBuffer
{
    public long Length => stream.Length;

    public long Position => stream.Position;

    public bool HasData => stream.Position < stream.Length;

    public static MinecraftBuffer Empty => new();

    private MemoryStream stream;

    public MinecraftBuffer()
    {
        stream = new();
    }

    public MinecraftBuffer(byte[] source)
    {
        stream = new(source);
    }

    public Memory<byte> AsMemory() => ToArray();

    public byte[] ToArray() => stream.ToArray();

    public void Clear() => stream = new();

    public byte ReadUnsignedByte()
    {
        var buffer = new byte[1];
        stream.Read(buffer);
        return buffer[0];
    }

    public void WriteUnsignedByte(byte value)
    {
        Write([value]);
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
            result |= value << (7 * numRead);

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
        var unsigned = (uint)value;

        do
        {
            var temp = (byte)(unsigned & 127);

            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            WriteUnsignedByte(temp);
        }
        while (unsigned != 0);
    }

    public int GetVarIntSize(int value)
    {
        return (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;
    }

    public ushort ReadUnsignedShort()
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadUInt16BigEndian(buffer);
    }

    public void WriteUnsignedShort(ushort value)
    {
        Span<byte> span = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        stream.Write(span);
    }

    public string ReadString(int maxLength = 32767)
    {
        var length = ReadVarInt();
        var buffer = new byte[length];

        if (stream.Read(buffer, 0, length) != length)
            throw new IndexOutOfRangeException();

        var value = Encoding.UTF8.GetString(buffer);

        if (maxLength > 0 && value.Length > maxLength)
            throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(value));

        return value;
    }

    public void WriteString(string value, int maxLength = short.MaxValue)
    {
        var bytes = new byte[Encoding.UTF8.GetByteCount(value)];
        Encoding.UTF8.GetBytes(value, bytes.AsSpan());
        WriteVarInt(bytes.Length);
        Write(bytes);
    }

    public byte[] ReadUInt8Array(int length)
    {
        var result = new byte[length];

        if (length == 0)
            return result;

        if (Read(result) != length)
            throw new IndexOutOfRangeException();

        return result;
    }

    public bool ReadBoolean()
    {
        return ReadUnsignedByte() == 0x01;
    }

    public void WriteBoolean(bool value)
    {
        stream.WriteByte((byte)(value ? 0x01 : 0x00));
    }

    public Guid ReadGuid() => GuidHelper.FromLongs(ReadLong(), ReadLong());

    public void WriteGuid(Guid value)
    {
        if (value == Guid.Empty)
        {
            WriteLong(0L);
            WriteLong(0L);
        }
        else
        {
            var uuid = BigInteger.Parse(value.ToString().Replace("-", ""), NumberStyles.HexNumber);
            Write(uuid.ToByteArray(false, true));
        }
    }

    public long ReadLong()
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadInt64BigEndian(buffer);
    }

    public void WriteLong(long value)
    {
        Span<byte> span = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        stream.Write(span);
    }

    public IdentifiedKey ReadIdentifiedKey() => new()
    {
        ExpiresAt = ReadLong(),
        PublicKey = ReadUInt8Array(ReadVarInt()),
        KeySignature = ReadUInt8Array(ReadVarInt())
    };

    public void WriteIdentifiedKey(IdentifiedKey identifiedKey)
    {
        WriteLong(identifiedKey.ExpiresAt);
        
        WriteVarInt(identifiedKey.PublicKey.Length);
        Write(identifiedKey.PublicKey);
        
        WriteVarInt(identifiedKey.KeySignature.Length);
        Write(identifiedKey.KeySignature);
    }

    public List<Property> ReadPropertyList(int count = -1) => Enumerable.Range(0, count < 0 ? ReadVarInt() : count).Select<int, Property>(_ =>
    {
        var name = ReadString();
        var value = ReadString();
        var isSigned = ReadBoolean();
        var signature = isSigned ? ReadString() : null;

        return new(name, value, isSigned, signature);
    }).ToList();

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

    public int Read(byte[] buffer) => stream.Read(buffer);

    public void Write(byte[] buffer) => stream.Write(buffer);
}