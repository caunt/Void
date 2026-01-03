using System;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Nbt;
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

    public short ReadShort()
    {
        return _backingBuffer.ReadShort();
    }

    public void WriteShort(short value)
    {
        _backingBuffer.WriteShort(value);
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

    public long ReadVarLong()
    {
        return _backingBuffer.ReadVarLong();
    }

    public void WriteVarLong(long value)
    {
        _backingBuffer.WriteVarLong(value);
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

    public double ReadDouble()
    {
        return _backingBuffer.ReadDouble();
    }

    public void WriteDouble(double value)
    {
        _backingBuffer.WriteDouble(value);
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

    public void WriteString(ReadOnlySpan<char> value)
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

    public void WritePropertyArray(Property[]? value)
    {
        _backingBuffer.WritePropertyArray(value ?? []);
    }

    public NbtTag ReadTag(bool readName = false)
    {
        return _backingBuffer.ReadTag(readName);
    }

    public void WriteTag(NbtTag value, bool writeName = true)
    {
        _backingBuffer.WriteTag(value, writeName);
    }

    public Component ReadComponent()
    {
        return Component.ReadFrom(ref this);
    }

    public Component ReadJsonComponent()
    {
        return Component.ReadJsonFrom(ref this);
    }

    public void WriteComponent(Component value, bool writeName = false)
    {
        value.WriteTo(ref this, writeName);
    }

    public void WriteJsonComponent(Component value, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        value.WriteJsonTo(ref this, jsonSerializerOptions);
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

    public ReadOnlySpan<byte> DumpBytes()
    {
        var position = Position;
        Seek(0, SeekOrigin.Begin);

        var data = ReadToEnd();
        Seek(position, SeekOrigin.Begin);

        return data;
    }

    public string DumpHex()
    {
        return Convert.ToHexString(DumpBytes());
    }

    public string Dump()
    {
        return $"Length: {Length}, Position: {Position}, Bytes: {DumpHex()}";
    }

    public void Reset()
    {
        _backingBuffer.Reset();
    }
}
