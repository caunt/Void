using System;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Nbt;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Buffers;

public ref struct MinecraftBuffer
{
    private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new()
    {
        WriteIndented = false
    };

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

    /// <summary>
    /// Reads a Minecraft protocol string encoded as a <c>VarInt</c> byte length followed by UTF-8 bytes.
    /// </summary>
    /// <param name="maxLength">
    /// Maximum allowed character count for the decoded string.
    /// </param>
    /// <returns>
    /// The decoded UTF-8 string. The returned value is never <see langword="null" />.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method validates the decoded character length, not the encoded byte length. If <paramref name="maxLength" /> is less than or equal to <c>0</c>, length validation is skipped.
    /// </para>
    /// <para>
    /// The buffer read position advances by the size of the string length prefix and payload bytes.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// The length prefix is not a valid Minecraft <c>VarInt</c> value.
    /// </exception>
    /// <exception cref="IndexOutOfRangeException">
    /// The decoded string length exceeds <paramref name="maxLength" />.
    /// </exception>
    /// <exception cref="System.Data.ReadOnlyException">
    /// Propagated from the underlying buffer implementation in unsupported scenarios.
    /// </exception>
    /// <example>
    /// <code>
    /// var text = buffer.ReadString(16);
    /// </code>
    /// </example>
    /// <see cref="WriteString(ReadOnlySpan{char})" />
    /// <seealso cref="ReadVarInt()" />
    public string ReadString(int maxLength = 32767)
    {
        return _backingBuffer.ReadString(maxLength);
    }

    /// <summary>
    /// Writes a Minecraft protocol string as UTF-8 bytes prefixed by its byte length encoded as <c>VarInt</c>.
    /// </summary>
    /// <param name="value">
    /// The characters to encode and write.
    /// </param>
    /// <remarks>
    /// <para>
    /// The method writes the UTF-8 byte count first, then writes the encoded bytes. An empty span writes a zero length prefix and no payload bytes.
    /// </para>
    /// <para>
    /// This method mutates the buffer by advancing the current position and appending or overwriting data depending on the backing storage.
    /// </para>
    /// </remarks>
    /// <exception cref="System.Data.ReadOnlyException">
    /// The underlying backing buffer is read-only.
    /// </exception>
    /// <exception cref="InternalBufferOverflowException">
    /// The target writable span does not have enough capacity for the encoded payload.
    /// </exception>
    /// <example>
    /// <code>
    /// buffer.WriteString("minecraft:stone");
    /// </code>
    /// </example>
    /// <see cref="ReadString(int)" />
    /// <seealso cref="WriteVarInt(int)" />
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

    public NbtTag ReadTag(bool readName = true)
    {
        return _backingBuffer.ReadTag(readName);
    }

    public void WriteTag(NbtTag value, bool writeName = true)
    {
        _backingBuffer.WriteTag(value, writeName);
    }

    public Component ReadComponent(bool asNbt = true)
    {
        return asNbt
            ? Component.ReadFrom(ref this, readName: false)
            : Component.DeserializeJson(ReadJsonString());
    }

    public void WriteComponent(Component value, bool asNbt = true, bool writeNbtName = false)
    {
        if (asNbt)
            value.WriteTo(ref this, writeNbtName);
        else
            WriteJsonString(value.SerializeJson());
    }

    public JsonNode ReadJsonString()
    {
        return JsonNode.Parse(ReadString()) ?? throw new InvalidDataException("Failed to parse JsonNode from buffer string.");
    }

    public void WriteJsonString(JsonNode node, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        WriteString(node.ToJsonString(jsonSerializerOptions ?? _defaultJsonSerializerOptions));
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

    public void Write(scoped ReadOnlySpan<byte> data)
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

    public BufferSpan CopyAsBufferSpan(bool read = false)
    {
        if (read)
            return new BufferSpan(ReadToEnd().ToArray());
        else
            return new BufferSpan(DumpBytes().ToArray()) { Position = (int)Position };
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
