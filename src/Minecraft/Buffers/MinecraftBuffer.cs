using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Nbt;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Buffers;

public ref struct MinecraftBuffer
{
    private MinecraftBackingBuffer _backingBuffer;

    public readonly bool HasData => _backingBuffer.HasData();
    public readonly long Position => _backingBuffer.GetPosition();
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
        return (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;
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

    public float ReadFloat()
    {
        return _backingBuffer.ReadFloat();
    }

    public void WriteFloat(float value)
    {
        _backingBuffer.WriteFloat(value);
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

    public NbtTag ReadTag()
    {
        return _backingBuffer.ReadTag();
    }

    public void WriteTag(NbtTag value)
    {
        _backingBuffer.WriteTag(value);
    }

    public Component ReadComponent(ProtocolVersion protocolVersion)
    {
        return Component.ReadFrom(ref this, protocolVersion);
    }

    public void WriteComponent(Component value, ProtocolVersion protocolVersion)
    {
        value.WriteTo(ref this, protocolVersion);
    }

    public void Seek(long offset)
    {
        Seek(offset, SeekOrigin.Begin);
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        _backingBuffer.Seek(offset, origin);
    }

    public ReadOnlySpan<byte> Read(long length)
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
}