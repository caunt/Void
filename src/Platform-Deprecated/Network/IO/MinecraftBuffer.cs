﻿using Minecraft.Component.Component;
using SharpNBT;
using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;
using System.Text;
using Void.Proxy.Models.Minecraft.Encryption;
using Void.Proxy.Models.Minecraft.Profile;
using Void.Proxy.Network.Protocol;
using Void.Proxy.Utils;

namespace Void.Proxy.Network.IO;

public ref struct MinecraftBuffer(Memory<byte> memory)
{
    public long Length => Span.Length;
    public bool HasData => Position < Length;
    public long Position { get; private set; }
    public Span<byte> Span { get; init; } = memory.Span;

    public MinecraftBuffer(int size) : this(MemoryPool<byte>.Shared.Rent(size).Memory)
    {
    } // is it safe? will GC dispose MemoryOwner too early?

    public MinecraftBuffer() : this(2048)
    {
    }

    public void Seek(long offset)
    {
        Seek(offset, SeekOrigin.Current);
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
            Position = offset;
        else if (origin == SeekOrigin.Current)
            Position += offset;
        else
            Position -= offset;

        if (Position < 0 || Position > Length)
            throw new IndexOutOfRangeException($"Position {Position}, Length {Length}");
    }

    public void Reset()
    {
        Position = 0;
    }

    public static int GetVarIntSize(int value)
    {
        return ((BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171) >> 13;
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
        } while (unsigned != 0);
    }

    public int ReadVarInt()
    {
        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            read = ReadUnsignedByte();
            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5)
                throw new InvalidOperationException("VarInt is too big");
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
        return Span[(int)Position++];
    }

    public void WriteUnsignedByte(byte value)
    {
        Span[(int)Position++] = value;
    }

    public ushort ReadUnsignedShort()
    {
        var span = Span.Slice((int)Position, 2);
        Position += 2;
        return BinaryPrimitives.ReadUInt16BigEndian(span);
    }

    public void WriteUnsignedShort(ushort value)
    {
        var span = Span.Slice((int)Position, 2);
        Position += 2;
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
    }

    public int ReadVarShort()
    {
        var low = ReadUnsignedShort();
        var high = 0;

        if ((low & 0x8000) != 0)
        {
            low &= 0x7FFF;
            high = ReadUnsignedByte();
        }

        return ((high & 0xFF) << 15) | low;
    }

    public void WriteVarShort(int value)
    {
        var low = (ushort)(value & 0x7FFF);
        var high = (byte)((value & 0x7F8000) >> 15);

        if (high != 0)
            low |= 0x8000;

        WriteUnsignedShort(low);

        if (high != 0)
            WriteUnsignedByte(high);
    }

    public string ReadString(int maxLength = 32767)
    {
        var length = ReadVarInt();
        var span = Span.Slice((int)Position, length);
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

        var span = Span.Slice((int)Position, length);
        Position += length;

        Encoding.UTF8.GetBytes(value, span);
    }

    public ChatComponent ReadComponent(int maxLength = 32767, ProtocolVersion? protocolVersion = null)
    {
        if (protocolVersion != null && protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
        {
            throw new NotImplementedException(); // how do we read exact array?
            var span = ReadToEnd();
            var stream = new MemoryStream();

            stream.Write(span);
            stream.Position = 0;

            using var reader = new TagReader(stream, FormatOptions.Java);
            var tag = reader.ReadTag(false);

            return ChatComponent.FromNbt(tag);
        }

        // read as json
        return ChatComponent.FromJson(ReadString(maxLength));
    }

    public void WriteComponent(ChatComponent component, ProtocolVersion? protocolVersion = null)
    {
        if (protocolVersion != null && protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
        {
            // issue SharpNBT to accept Span<byte> instead of only streams
            var stream = new MemoryStream();
            using var writer = new TagWriter(stream, FormatOptions.Java);
            var tag = component.ToNbt();

            // issue SharpNBT to write TAG type even if tag is unnamed
            stream.WriteByte((byte)tag.Type);
            writer.WriteTag(tag);

            Write(stream.ToArray());
        }
        else
        {
            // write as json
            WriteString(component.ToString());
        }
    }

    public bool ReadBoolean()
    {
        return ReadUnsignedByte() == 0x01;
    }

    public void WriteBoolean(bool value)
    {
        WriteUnsignedByte((byte)(value ? 0x01 : 0x00));
    }

    public int ReadInt()
    {
        var span = Span.Slice((int)Position, 4);
        Position += 4;
        return BitConverter.ToInt32(span);
    }

    public void WriteInt(int value)
    {
        var span = Span.Slice((int)Position, 4);
        Position += 4;
        BitConverter.GetBytes(value).CopyTo(span);
    }

    public Guid ReadGuid()
    {
        return GuidHelper.FromLongs(ReadLong(), ReadLong());
    }

    public void WriteGuid(Guid value)
    {
        if (value == Guid.Empty)
            Write(new byte[16]);
        else
            Write(value.ToByteArray(true));
    }

    public Guid ReadGuidIntArray()
    {
        var msbHigh = (long)ReadInt() << 32;
        var msbLow = ReadInt() & 0xFFFFFFFFL;
        var msb = msbHigh | msbLow;
        var lsbHigh = (long)ReadInt() << 32;
        var lsbLow = ReadInt() & 0xFFFFFFFFL;
        var lsb = lsbHigh | lsbLow;

        return GuidHelper.FromLongs(msb, lsb);
    }

    public void WriteGuidIntArray(Guid value)
    {
        var bytes = value.ToByteArray();
        var msb = BitConverter.ToUInt64(bytes, 0);
        var lsb = BitConverter.ToUInt64(bytes, 8);

        WriteInt((int)(msb >> 32));
        WriteInt((int)msb);
        WriteInt((int)(lsb >> 32));
        WriteInt((int)lsb);
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public void WriteLong(long value)
    {
        var span = Span.Slice((int)Position, 8);
        Position += 8;
        BinaryPrimitives.WriteInt64BigEndian(span, value);
    }

    public IdentifiedKey ReadIdentifiedKey(ProtocolVersion protocolVersion)
    {
        return new IdentifiedKey(protocolVersion == ProtocolVersion.MINECRAFT_1_19 ? IdentifiedKeyRevision.GENERIC_V1 : IdentifiedKeyRevision.LINKED_V2, ReadLong(), Read(ReadVarInt()).ToArray(), Read(ReadVarInt()).ToArray());
    }

    public void WriteIdentifiedKey(IdentifiedKey identifiedKey)
    {
        WriteLong(identifiedKey.ExpiresAt);

        WriteVarInt(identifiedKey.PublicKey.Length);
        Write(identifiedKey.PublicKey);

        WriteVarInt(identifiedKey.Signature.Length);
        Write(identifiedKey.Signature);
    }

    public List<Property> ReadPropertyList(int count = -1)
    {
        if (count < 0)
            count = ReadVarInt();

        var list = new List<Property>();

        for (var i = 0; i < count; i++)
        {
            var name = ReadString();
            var value = ReadString();
            var isSigned = ReadBoolean();
            var signature = isSigned ? ReadString() : null;

            list.Add(new Property(name, value, isSigned, signature));
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

    public Span<byte> ReadToEnd()
    {
        return Read((int)Length - (int)Position);
    }

    public Span<byte> Read(int length)
    {
        if (Length < Position + length)
            throw new InternalBufferOverflowException($"Buffer length with max {Length} at position {Position} attempted to read {length} bytes");

        var span = Span.Slice((int)Position, length);
        Position += length;
        return span;
    }

    public void Write(Span<byte> data)
    {
        if (Length < Position + data.Length)
            throw new InternalBufferOverflowException($"Buffer length with max {Length} at position {Position} attempted to write {data.Length} bytes");

        var span = Span.Slice((int)Position, data.Length);
        Position += data.Length;
        data.CopyTo(span);
    }
}