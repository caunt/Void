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

/// <summary>
/// Provides cursor-based reading and writing of Minecraft protocol values over span, sequence, or stream-backed storage.
/// </summary>
/// <remarks>
/// Read and write operations advance <see cref="Position" />. Buffers created from <see cref="ReadOnlySpan{T}" /> or <see cref="ReadOnlySequence{T}" /> reject writes, while a writable span has fixed capacity and a <see cref="MemoryStream" /> follows the capabilities of that stream.
/// </remarks>
public ref struct MinecraftBuffer
{
    private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new()
    {
        WriteIndented = false
    };

    private MinecraftBackingBuffer _backingBuffer;

    /// <summary>
    /// Gets whether unread bytes remain after the current position.
    /// </summary>
    public readonly bool HasData => _backingBuffer.HasData();

    /// <summary>
    /// Gets the current zero-based byte position.
    /// </summary>
    public readonly long Position => _backingBuffer.GetPosition();

    /// <summary>
    /// Gets the current length of the backing storage in bytes.
    /// </summary>
    public readonly long Length => _backingBuffer.GetLength();

    /// <summary>
    /// Prevents creation of an uninitialized buffer.
    /// </summary>
    /// <exception cref="NotSupportedException">Always thrown because a backing store is required.</exception>
    public MinecraftBuffer() => throw new NotSupportedException("Parameterless constructor not supported");

    /// <summary>
    /// Initializes a readable and writable fixed-capacity buffer over a span.
    /// </summary>
    /// <param name="memory">The backing span, retained for the lifetime of this stack-only buffer.</param>
    public MinecraftBuffer(Span<byte> memory)
    {
        _backingBuffer = new MinecraftBackingBuffer(memory);
    }

    /// <summary>
    /// Initializes a read-only buffer over a span.
    /// </summary>
    /// <param name="span">The backing read-only span.</param>
    public MinecraftBuffer(ReadOnlySpan<byte> span)
    {
        _backingBuffer = new MinecraftBackingBuffer(span);
    }

    /// <summary>
    /// Initializes a read-only buffer over a possibly segmented byte sequence.
    /// </summary>
    /// <param name="sequence">The backing sequence, retained without copying.</param>
    public MinecraftBuffer(ReadOnlySequence<byte> sequence)
    {
        _backingBuffer = new MinecraftBackingBuffer(sequence);
    }

    /// <summary>
    /// Initializes a buffer over an existing memory stream at its current position.
    /// </summary>
    /// <param name="memoryStream">The backing stream, retained without transferring disposal ownership.</param>
    public MinecraftBuffer(MemoryStream memoryStream)
    {
        _backingBuffer = new MinecraftBackingBuffer(memoryStream);
    }

    /// <summary>
    /// Calculates the number of bytes required by Minecraft's variable-length encoding of a 32-bit integer.
    /// </summary>
    /// <param name="value">The integer to measure.</param>
    /// <returns>A value from <c>1</c> through <c>5</c>.</returns>
    public static int GetVarIntSize(int value)
    {
        return (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;
    }

    /// <summary>
    /// Reads one unsigned byte and advances the position by one.
    /// </summary>
    /// <returns>The byte read from the current position.</returns>
    public byte ReadUnsignedByte()
    {
        return _backingBuffer.ReadUnsignedByte();
    }

    /// <summary>
    /// Writes one unsigned byte and advances the position by one.
    /// </summary>
    /// <param name="value">The byte to write.</param>
    public void WriteUnsignedByte(byte value)
    {
        _backingBuffer.WriteUnsignedByte(value);
    }

    /// <summary>
    /// Reads one byte and converts it to a Boolean value.
    /// </summary>
    /// <returns><see langword="false" /> for zero; otherwise, <see langword="true" />.</returns>
    public bool ReadBoolean()
    {
        return _backingBuffer.ReadBoolean();
    }

    /// <summary>
    /// Writes a Boolean as <c>0</c> or <c>1</c> in one byte.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteBoolean(bool value)
    {
        _backingBuffer.WriteBoolean(value);
    }

    /// <summary>
    /// Reads an unsigned 16-bit integer in big-endian order.
    /// </summary>
    /// <returns>The decoded value.</returns>
    public ushort ReadUnsignedShort()
    {
        return _backingBuffer.ReadUnsignedShort();
    }

    /// <summary>
    /// Writes an unsigned 16-bit integer to the buffer using Minecraft's big-endian binary encoding.
    /// </summary>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <remarks>
    /// This method writes exactly two bytes and advances the current buffer position by two.
    /// </remarks>
    /// <exception cref="System.Data.ReadOnlyException">
    /// The underlying backing buffer does not support writes.
    /// </exception>
    /// <exception cref="InternalBufferOverflowException">
    /// The target writable span does not have enough capacity for the encoded value.
    /// </exception>
    public void WriteUnsignedShort(ushort value)
    {
        _backingBuffer.WriteUnsignedShort(value);
    }

    /// <summary>
    /// Reads a signed 16-bit integer in big-endian order.
    /// </summary>
    /// <returns>The decoded value.</returns>
    public short ReadShort()
    {
        return _backingBuffer.ReadShort();
    }

    /// <summary>
    /// Writes a signed 16-bit integer to the buffer using Minecraft's big-endian binary encoding.
    /// </summary>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method writes exactly two bytes and advances the current buffer position by two.
    /// </para>
    /// <para>
    /// The value is written in network order, which matches the encoding used by the Minecraft protocol.
    /// </para>
    /// </remarks>
    /// <exception cref="System.Data.ReadOnlyException">
    /// The underlying backing buffer does not support writes.
    /// </exception>
    /// <exception cref="InternalBufferOverflowException">
    /// The target writable span does not have enough capacity for the encoded value.
    /// </exception>
    public void WriteShort(short value)
    {
        _backingBuffer.WriteShort(value);
    }

    /// <summary>
    /// Reads the protocol's one-to-three-byte variable short representation.
    /// </summary>
    /// <returns>The decoded nonnegative integer value.</returns>
    public int ReadVarShort()
    {
        return _backingBuffer.ReadVarShort();
    }

    /// <summary>
    /// Writes the low 23 bits of an integer using the protocol's variable short representation.
    /// </summary>
    /// <param name="value">The value whose low 23 bits are written.</param>
    public void WriteVarShort(int value)
    {
        _backingBuffer.WriteVarShort(value);
    }

    /// <summary>
    /// Reads a Minecraft variable-length signed 32-bit integer.
    /// </summary>
    /// <returns>The decoded integer.</returns>
    /// <exception cref="InvalidOperationException">The encoding uses more than five bytes.</exception>
    public int ReadVarInt()
    {
        return _backingBuffer.ReadVarInt();
    }

    /// <summary>
    /// Writes a signed 32-bit integer using Minecraft variable-length encoding.
    /// </summary>
    /// <param name="value">The integer to write.</param>
    public void WriteVarInt(int value)
    {
        _backingBuffer.WriteVarInt(value);
    }

    /// <summary>
    /// Reads a Minecraft variable-length signed 64-bit integer.
    /// </summary>
    /// <returns>The decoded integer.</returns>
    /// <exception cref="InvalidOperationException">The encoding uses more than ten bytes.</exception>
    public long ReadVarLong()
    {
        return _backingBuffer.ReadVarLong();
    }

    /// <summary>
    /// Writes a signed 64-bit integer using Minecraft variable-length encoding.
    /// </summary>
    /// <param name="value">The integer to write.</param>
    public void WriteVarLong(long value)
    {
        _backingBuffer.WriteVarLong(value);
    }

    /// <summary>
    /// Reads a signed 32-bit integer in big-endian order.
    /// </summary>
    /// <returns>The decoded value.</returns>
    public int ReadInt()
    {
        return _backingBuffer.ReadInt();
    }

    /// <summary>
    /// Writes a signed 32-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteInt(int value)
    {
        _backingBuffer.WriteInt(value);
    }

    /// <summary>
    /// Reads an IEEE 754 single-precision value in big-endian byte order.
    /// </summary>
    /// <returns>The decoded value.</returns>
    public float ReadFloat()
    {
        return _backingBuffer.ReadFloat();
    }

    /// <summary>
    /// Writes an IEEE 754 single-precision value in big-endian byte order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteFloat(float value)
    {
        _backingBuffer.WriteFloat(value);
    }

    /// <summary>
    /// Reads an IEEE 754 double-precision value in big-endian byte order.
    /// </summary>
    /// <returns>The decoded value.</returns>
    public double ReadDouble()
    {
        return _backingBuffer.ReadDouble();
    }

    /// <summary>
    /// Writes an IEEE 754 double-precision value in big-endian byte order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteDouble(double value)
    {
        _backingBuffer.WriteDouble(value);
    }

    /// <summary>
    /// Reads a signed 64-bit integer in big-endian order.
    /// </summary>
    /// <returns>The decoded value.</returns>
    public long ReadLong()
    {
        return _backingBuffer.ReadLong();
    }

    /// <summary>
    /// Writes a signed 64-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteLong(long value)
    {
        _backingBuffer.WriteLong(value);
    }

    /// <summary>
    /// Reads a UUID as two big-endian 64-bit protocol values.
    /// </summary>
    /// <returns>The decoded UUID.</returns>
    public Uuid ReadUuid()
    {
        return _backingBuffer.ReadUuid();
    }

    /// <summary>
    /// Writes a UUID as two big-endian 64-bit protocol values.
    /// </summary>
    /// <param name="value">The UUID to write.</param>
    public void WriteUuid(Uuid value)
    {
        _backingBuffer.WriteUuid(value);
    }

    /// <summary>
    /// Reads a UUID from the four-integer representation used by some protocol fields.
    /// </summary>
    /// <returns>The decoded UUID.</returns>
    public Uuid ReadUuidAsIntArray()
    {
        return _backingBuffer.ReadUuidAsIntArray();
    }

    /// <summary>
    /// Writes a UUID using its four-integer representation.
    /// </summary>
    /// <param name="value">The UUID to write.</param>
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

    /// <summary>
    /// Reads a length-prefixed profile property and its optional signature.
    /// </summary>
    /// <returns>The decoded profile property.</returns>
    public Property ReadProperty()
    {
        return _backingBuffer.ReadProperty();
    }

    /// <summary>
    /// Writes a profile property and its optional signature.
    /// </summary>
    /// <param name="value">The property to write.</param>
    /// <exception cref="InvalidDataException"><paramref name="value" /> is marked as signed but has a null, empty, or whitespace signature.</exception>
    public void WriteProperty(Property value)
    {
        _backingBuffer.WriteProperty(value);
    }

    /// <summary>
    /// Reads a variable-length count followed by that many profile properties.
    /// </summary>
    /// <returns>A newly allocated array containing the decoded properties.</returns>
    public Property[] ReadPropertyArray()
    {
        return _backingBuffer.ReadPropertyArray();
    }

    /// <summary>
    /// Writes a profile property array using Minecraft's length-prefixed format.
    /// </summary>
    /// <param name="value">
    /// The properties to serialize. When <see langword="null"/>, an empty array is written.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method writes a VarInt element count first, then serializes each <see cref="Property"/> by calling
    /// <see cref="WriteProperty(Property)"/>.
    /// </para>
    /// <para>
    /// Passing <see langword="null"/> is equivalent to passing <c>[]</c>, so the written count is <c>0</c>.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidDataException">
    /// Propagated from <see cref="WriteProperty(Property)"/> when a property is marked as signed but has a missing
    /// or whitespace signature.
    /// </exception>
    /// <example>
    /// <code>
    /// buffer.WritePropertyArray(profile.Properties);
    /// </code>
    /// </example>
    /// <see cref="WriteProperty(Property)" />
    /// <seealso cref="ReadPropertyArray()" />
    public void WritePropertyArray(Property[]? value)
    {
        _backingBuffer.WritePropertyArray(value ?? []);
    }

    /// <summary>
    /// Reads one binary NBT tag from the current position.
    /// </summary>
    /// <param name="readName"><see langword="true" /> to read the root tag name; otherwise, <see langword="false" />.</param>
    /// <returns>The parsed tag. Only the bytes consumed by that tag are removed from the unread region.</returns>
    public NbtTag ReadTag(bool readName = true)
    {
        return _backingBuffer.ReadTag(readName);
    }

    /// <summary>
    /// Serializes and writes one binary NBT tag.
    /// </summary>
    /// <param name="value">The tag to serialize.</param>
    /// <param name="writeName"><see langword="true" /> to include the root tag name; otherwise, <see langword="false" />.</param>
    public void WriteTag(NbtTag value, bool writeName = true)
    {
        _backingBuffer.WriteTag(value, writeName);
    }

    /// <summary>
    /// Reads a text component encoded as unnamed NBT or a length-prefixed JSON string.
    /// </summary>
    /// <param name="asNbt"><see langword="true" /> to read unnamed binary NBT; <see langword="false" /> to parse JSON text.</param>
    /// <returns>The deserialized component.</returns>
    public Component ReadComponent(bool asNbt = true)
    {
        return asNbt
            ? Component.ReadFrom(ref this, readName: false)
            : Component.DeserializeJson(ReadJsonString());
    }

    /// <summary>
    /// Writes a text component as binary NBT or a length-prefixed JSON string.
    /// </summary>
    /// <param name="value">The component to serialize.</param>
    /// <param name="asNbt"><see langword="true" /> to use binary NBT; <see langword="false" /> to use JSON.</param>
    /// <param name="writeNbtName">When writing NBT, whether to include the root tag name.</param>
    public void WriteComponent(Component value, bool asNbt = true, bool writeNbtName = false)
    {
        if (asNbt)
            value.WriteTo(ref this, writeNbtName);
        else
            WriteJsonString(value.SerializeJson());
    }

    /// <summary>
    /// Reads a length-prefixed UTF-8 string and parses it as a JSON node.
    /// </summary>
    /// <returns>The parsed JSON node.</returns>
    /// <exception cref="InvalidDataException">Parsing produces a null JSON node.</exception>
    public JsonNode ReadJsonString()
    {
        return JsonNode.Parse(ReadString()) ?? throw new InvalidDataException("Failed to parse JsonNode from buffer string.");
    }

    /// <summary>
    /// Serializes a JSON node and writes it as a length-prefixed UTF-8 string.
    /// </summary>
    /// <param name="node">The node to serialize.</param>
    /// <param name="jsonSerializerOptions">Optional serializer options; <see langword="null" /> uses compact default output.</param>
    public void WriteJsonString(JsonNode node, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        WriteString(node.ToJsonString(jsonSerializerOptions ?? _defaultJsonSerializerOptions));
    }

    /// <summary>
    /// Sets the position to an absolute byte offset from the beginning.
    /// </summary>
    /// <param name="offset">The new absolute position.</param>
    public void Seek(long offset)
    {
        Seek(offset, SeekOrigin.Begin);
    }

    /// <summary>
    /// Moves the position relative to a selected origin.
    /// </summary>
    /// <param name="offset">The signed byte offset from <paramref name="origin" />.</param>
    /// <param name="origin">The reference point used to calculate the new position.</param>
    public void Seek(long offset, SeekOrigin origin)
    {
        _backingBuffer.Seek(offset, origin);
    }

    /// <summary>
    /// Reads a contiguous span of bytes and advances the position.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>A read-only span containing the requested bytes.</returns>
    public ReadOnlySpan<byte> Read(long length)
    {
        return _backingBuffer.Read(length);
    }

    /// <summary>
    /// Writes a byte span and advances the position by its length.
    /// </summary>
    /// <param name="data">The bytes to write.</param>
    public void Write(scoped ReadOnlySpan<byte> data)
    {
        _backingBuffer.Write(data);
    }

    /// <summary>
    /// Copies bytes from a stream's current position into this buffer.
    /// </summary>
    /// <param name="stream">The source stream. This method does not dispose it.</param>
    public void Write(Stream stream)
    {
        _backingBuffer.Write(stream);
    }

    /// <summary>
    /// Reads all bytes from the current position through the current length.
    /// </summary>
    /// <returns>The unread bytes; the position advances to the end.</returns>
    public ReadOnlySpan<byte> ReadToEnd()
    {
        return _backingBuffer.ReadToEnd();
    }

    /// <summary>
    /// Returns all bytes from the beginning through the current length without changing the final position.
    /// </summary>
    /// <returns>A view of the complete buffer contents.</returns>
    public ReadOnlySpan<byte> DumpBytes()
    {
        var position = Position;
        Seek(0, SeekOrigin.Begin);

        var data = ReadToEnd();
        Seek(position, SeekOrigin.Begin);

        return data;
    }

    /// <summary>
    /// Copies buffer bytes into a newly allocated writable <see cref="BufferSpan" />.
    /// </summary>
    /// <param name="read"><see langword="true" /> to copy only unread bytes and advance this buffer to the end; <see langword="false" /> to copy all bytes and preserve this buffer's position in the returned copy.</param>
    /// <returns>A buffer span backed by a new byte array.</returns>
    public BufferSpan CopyAsBufferSpan(bool read = false)
    {
        if (read)
            return new BufferSpan(ReadToEnd().ToArray());
        else
            return new BufferSpan(DumpBytes().ToArray()) { Position = (int)Position };
    }

    /// <summary>
    /// Formats all buffer bytes as an uppercase hexadecimal string without changing the final position.
    /// </summary>
    /// <returns>The complete buffer contents in hexadecimal form.</returns>
    public string DumpHex()
    {
        return Convert.ToHexString(DumpBytes());
    }

    /// <summary>
    /// Formats the current length, position, and complete hexadecimal contents for diagnostics.
    /// </summary>
    /// <returns>A diagnostic buffer description.</returns>
    public string Dump()
    {
        return $"Length: {Length}, Position: {Position}, Bytes: {DumpHex()}";
    }

    /// <summary>
    /// Resets the backing buffer's position to its initial position.
    /// </summary>
    public void Reset()
    {
        _backingBuffer.Reset();
    }
}
