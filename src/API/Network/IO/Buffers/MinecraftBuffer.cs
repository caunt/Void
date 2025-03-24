using System.Buffers;
using System.Numerics;
using Void.Proxy.Api.Mojang;
using Void.Proxy.Api.Mojang.Profiles;

namespace Void.Proxy.Api.Network.IO.Buffers;

public ref struct MinecraftBuffer
{
    private MinecraftBackingBuffer _backingBuffer;

    public readonly bool HasData => _backingBuffer.HasData();
    public readonly int Position => _backingBuffer.GetPosition();
    public readonly long Length => _backingBuffer.GetLength();

    public MinecraftBuffer() => throw new NotSupportedException("Parameterless constructor not supported");

    public MinecraftBuffer(Span<byte> memory)
    {
        _backingBuffer = new MinecraftBackingBuffer(memory);
    }

    public MinecraftBuffer(ReadOnlySpan<byte> span)
    {
        _backingBuffer = new MinecraftBackingBuffer(span);
    }

    public MinecraftBuffer(ReadOnlySequence<byte> sequence)
    {
        _backingBuffer = new MinecraftBackingBuffer(sequence);
    }

    public MinecraftBuffer(MemoryStream memoryStream)
    {
        _backingBuffer = new MinecraftBackingBuffer(memoryStream);
    }

    public static int GetVarIntSize(int value)
    {
        return ((BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171) >> 13;
    }

    public static IEnumerable<byte> EnumerateVarInt(int value)
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

    public byte ReadUnsignedByte()
    {
        return _backingBuffer.ReadUnsignedByte();
    }

    public void WriteUnsignedByte(byte value)
    {
        _backingBuffer.WriteUnsignedByte(value);
    }

    public bool ReadBoolean()
    {
        return _backingBuffer.ReadBoolean();
    }

    public void WriteBoolean(bool value)
    {
        _backingBuffer.WriteBoolean(value);
    }

    public ushort ReadUnsignedShort()
    {
        return _backingBuffer.ReadUnsignedShort();
    }

    public void WriteUnsignedShort(ushort value)
    {
        _backingBuffer.WriteUnsignedShort(value);
    }

    public int ReadVarShort()
    {
        return _backingBuffer.ReadVarShort();
    }

    public void WriteVarShort(int value)
    {
        _backingBuffer.WriteVarShort(value);
    }

    public int ReadVarInt()
    {
        return _backingBuffer.ReadVarInt();
    }

    public void WriteVarInt(int value)
    {
        _backingBuffer.WriteVarInt(value);
    }

    public int ReadInt()
    {
        return _backingBuffer.ReadInt();
    }

    public void WriteInt(int value)
    {
        _backingBuffer.WriteInt(value);
    }

    public long ReadLong()
    {
        return _backingBuffer.ReadLong();
    }

    public void WriteLong(long value)
    {
        _backingBuffer.WriteLong(value);
    }

    public Uuid ReadUuid()
    {
        return _backingBuffer.ReadUuid();
    }

    public void WriteUuid(Uuid value)
    {
        _backingBuffer.WriteUuid(value);
    }

    public Uuid ReadUuidAsIntArray()
    {
        return _backingBuffer.ReadUuidAsIntArray();
    }

    public void WriteUuidAsIntArray(Uuid value)
    {
        _backingBuffer.WriteUuidAsIntArray(value);
    }

    public string ReadString(int maxLength = 32767)
    {
        return _backingBuffer.ReadString(maxLength);
    }

    public void WriteString(string value)
    {
        _backingBuffer.WriteString(value);
    }

    public Property ReadProperty()
    {
        return _backingBuffer.ReadProperty();
    }

    public void WriteProperty(Property value)
    {
        _backingBuffer.WriteProperty(value);
    }

    public Property[] ReadPropertyArray()
    {
        return _backingBuffer.ReadPropertyArray();
    }

    public void WritePropertyArray(Property[] value)
    {
        _backingBuffer.WritePropertyArray(value);
    }

    public void Seek(int offset)
    {
        Seek(offset, SeekOrigin.Begin);
    }

    public void Seek(int offset, SeekOrigin origin)
    {
        _backingBuffer.Seek(offset, origin);
    }

    public ReadOnlySpan<byte> Read(int length)
    {
        return _backingBuffer.Read(length);
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        _backingBuffer.Write(data);
    }

    public void Write(Stream stream)
    {
        _backingBuffer.Write(stream);
    }

    public ReadOnlySpan<byte> ReadToEnd()
    {
        return _backingBuffer.ReadToEnd();
    }

    public void Reset()
    {
        _backingBuffer.Reset();
    }

    /*public ChatComponent ReadComponent(int maxLength = 32767, ProtocolVersion? protocolVersion = null)
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
        else
        {
            // read as json
            return ChatComponent.FromJson(ReadString(maxLength));
        }
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
    }*/

    // public Guid ReadGuid() => GuidHelper.FromLongs(ReadLong(), ReadLong());

    /*public Guid ReadGuidIntArray()
    {
        long msbHigh = (long)ReadInt() << 32;
        long msbLow = (long)ReadInt() & 0xFFFFFFFFL;
        long msb = msbHigh | msbLow;
        long lsbHigh = (long)ReadInt() << 32;
        long lsbLow = (long)ReadInt() & 0xFFFFFFFFL;
        long lsb = lsbHigh | lsbLow;

        return GuidHelper.FromLongs(msb, lsb);
    }*/
}