using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Nbt;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Buffers.Extensions;

public static class WriteMinecraftBufferExtensions
{
    /// <summary>
    /// Writes a single byte value to a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The buffer is modified to store the byte value at a designated position.</param>
    /// <param name="value">The byte value to be written into the buffer.</param>
    public static void WriteUnsignedByte<TBuffer>(ref this TBuffer buffer, byte value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.AccessWrite(1)[0] = value;

    /// <summary>
    /// Writes a boolean value to a specified buffer by converting it to a byte.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">The buffer is the destination where the boolean value will be written.</param>
    /// <param name="value">The boolean value to be converted to a byte and written to the buffer.</param>
    public static void WriteBoolean<TBuffer>(ref this TBuffer buffer, bool value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.WriteUnsignedByte(Convert.ToByte(value));

    /// <summary>
    /// Writes a variable-length short integer to the specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a struct that implements a specific buffer interface for Minecraft data handling.</typeparam>
    /// <param name="buffer">The buffer is the destination where the variable-length short integer will be written.</param>
    /// <param name="value">The integer value to be written to the buffer in a variable-length format.</param>
    public static void WriteVarShort<TBuffer>(ref this TBuffer buffer, int value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        WriteVarShortCore(ref buffer, value);

    /// <summary>
    /// Writes an unsigned short value to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for writing operations.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the unsigned short value will be written.</param>
    /// <param name="value">This parameter is the unsigned short value that will be written to the buffer.</param>
    public static void WriteUnsignedShort<TBuffer>(ref this TBuffer buffer, ushort value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.WriteUInt16BigEndian(buffer.AccessWrite(2), value);

    /// <summary>
    /// Writes a short integer to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the short integer will be written.</param>
    /// <param name="value">This parameter is the short integer value to be written to the buffer.</param>
    public static void WriteShort<TBuffer>(ref this TBuffer buffer, short value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.WriteInt16BigEndian(buffer.AccessWrite(2), value);

    public static void WriteVarInt<TBuffer>(ref this TBuffer buffer, int value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        WriteVarIntCore(ref buffer, value);

    /// <summary>
    /// Writes a 32-bit integer to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a struct that implements a specific buffer interface, allowing for efficient memory access.</typeparam>
    /// <param name="buffer">The buffer is modified to include the new integer value at the appropriate position.</param>
    /// <param name="value">The integer to be written into the buffer in big-endian format.</param>
    public static void WriteInt<TBuffer>(ref this TBuffer buffer, int value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.WriteInt32BigEndian(buffer.AccessWrite(4), value);

    /// <summary>
    /// Writes a floating-point number in big-endian format to a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a structure that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">The buffer is modified to store the floating-point value in a specific format.</param>
    /// <param name="value">The floating-point number to be written into the buffer.</param>
    public static void WriteFloat<TBuffer>(ref this TBuffer buffer, float value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.WriteSingleBigEndian(buffer.AccessWrite(4), value);

    /// <summary>
    /// Writes a double value to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the double value will be written.</param>
    /// <param name="value">This parameter is the double value that will be written to the buffer.</param>
    public static void WriteDouble<TBuffer>(ref this TBuffer buffer, double value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.WriteDoubleBigEndian(buffer.AccessWrite(8), value);

    /// <summary>
    /// Writes a variable-length long integer to a buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a struct that implements a specific buffer interface for Minecraft data handling.</typeparam>
    /// <param name="buffer">The buffer to which the variable-length long integer will be written.</param>
    /// <param name="value">The long integer value that will be written to the buffer.</param>
    public static void WriteVarLong<TBuffer>(ref this TBuffer buffer, long value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        WriteVarLongCore(ref buffer, value);

    /// <summary>
    /// Writes a 64-bit integer to a specified buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a struct that implements a specific buffer interface for writing data.</typeparam>
    /// <param name="buffer">The buffer is modified to accommodate the new data being written.</param>
    /// <param name="value">The 64-bit integer to be written into the buffer.</param>
    public static void WriteLong<TBuffer>(ref this TBuffer buffer, long value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.WriteInt64BigEndian(buffer.AccessWrite(8), value);

    /// <summary>
    /// Writes a UUID value to a specified buffer in a Minecraft-compatible format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    /// handling.</typeparam>
    /// <param name="buffer">The buffer is the destination where the UUID will be written.</param>
    /// <param name="value">The UUID value to be converted and written to the buffer.</param>
    public static void WriteUuid<TBuffer>(ref this TBuffer buffer, Uuid value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.Write(value.AsGuid.ToByteArray(true));

    /// <summary>
    /// Writes a UUID as an integer array into a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for Minecraft.</typeparam>
    /// <param name="buffer">The buffer is modified to include the integer representation of the UUID.</param>
    /// <param name="value">The UUID is converted and written into the buffer as an array of integers.</param>
    public static void WriteUuidAsIntArray<TBuffer>(ref this TBuffer buffer, Uuid value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        WriteUuidAsIntArrayCore(ref buffer, value);

    /// <summary>
    /// Writes a string to a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for Minecraft.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the string will be written.</param>
    /// <param name="value">This parameter is the string that will be written to the buffer.</param>
    public static void WriteString<TBuffer>(ref this TBuffer buffer, string value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        WriteStringCore(ref buffer, value);

    /// <summary>
    /// Writes a property to a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    /// handling.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the property will be written.</param>
    /// <param name="value">This parameter represents the property that is to be written to the buffer.</param>
    public static void WriteProperty<TBuffer>(ref this TBuffer buffer, Property value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        WritePropertyCore(ref buffer, value);

    /// <summary>
    /// Writes an array of properties to a specified buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a structure that implements a specific buffer interface for Minecraft data handling.</typeparam>
    /// <param name="buffer">The buffer to which the array of properties will be written.</param>
    /// <param name="value">The array of properties that will be written to the buffer.</param>
    public static void WritePropertyArray<TBuffer>(ref this TBuffer buffer, Property[] value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        WritePropertyArrayCore(ref buffer, value);

    /// <summary>
    /// Writes a tag to a specified buffer using a stream representation of the tag.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    /// handling.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the tag will be written.</param>
    /// <param name="value">This parameter represents the tag that will be converted to a stream and written to the buffer.</param>
    public static void WriteTag<TBuffer>(ref this TBuffer buffer, NbtTag value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.Write(value.AsStream());

    /// <summary>
    /// Writes a component to a specified buffer using a given protocol version.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    /// handling.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer where the component will be written.</param>
    /// <param name="value">This parameter represents the component that will be written to the buffer.</param>
    /// <param name="protocolVersion">This parameter indicates the version of the protocol to be used during the write operation.</param>
    public static void WriteComponent<TBuffer>(ref this TBuffer buffer, Component value, ProtocolVersion protocolVersion)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        value.WriteTo(ref buffer, protocolVersion);

    /// <summary>
    /// Writes a byte span to a buffer, allowing for efficient data handling in a structured format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a struct that implements a specific buffer interface for Minecraft data
    /// operations.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer that will receive the byte data.</param>
    /// <param name="data">This parameter contains the byte data that will be copied into the buffer.</param>
    public static void Write<TBuffer>(ref this TBuffer buffer, scoped ReadOnlySpan<byte> data)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        data.CopyTo(buffer.AccessWrite(data.Length));

    /// <summary>
    /// Writes data from a stream to a specified buffer. Requires the stream to support length property.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that can be used as a buffer for writing data.</typeparam>
    /// <param name="buffer">The buffer is a reference to the structure that will receive the data from the stream.</param>
    /// <param name="data">The stream provides the data source that will be written into the buffer.</param>
    public static void Write<TBuffer>(ref this TBuffer buffer, Stream value)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        value.ReadExactly(buffer.AccessWrite((int)value.Length));

    private static Span<byte> AccessWrite<TBuffer>(ref this TBuffer buffer, int length) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var span = buffer.Access(length);
        buffer.Seek(length);
        return span;
    }

    private static void WriteVarShortCore<TBuffer>(ref TBuffer buffer, int value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var low = (ushort)(value & 0x7FFF);
        var high = (byte)((value & 0x7F8000) >> 15);

        if (high != 0)
            low |= 0x8000;

        buffer.WriteUnsignedShort(low);

        if (high != 0)
            buffer.WriteUnsignedByte(high);
    }

    private static void WriteVarIntCore<TBuffer>(ref TBuffer buffer, int value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        Span<byte> valueBuffer = stackalloc byte[5];
        var length = value.AsVarInt(valueBuffer);
        buffer.Write(valueBuffer[..length]);
    }

    private static void WriteVarLongCore<TBuffer>(ref TBuffer buffer, long value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        while (true)
        {
            if ((value & ~0x7F) == 0)
            {
                buffer.WriteUnsignedByte((byte)value);
                return;
            }

            buffer.WriteUnsignedByte((byte)((value & 0x7F) | 0x80));
            value >>>= 7;
        }
    }

    private static void WriteUuidAsIntArrayCore<TBuffer>(ref TBuffer buffer, Uuid value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var span = value.AsGuid.ToByteArray().AsSpan();

        buffer.WriteInt(BitConverter.ToInt32(span[..4]));
        buffer.WriteInt(BitConverter.ToInt32(span[4..8]));
        buffer.WriteInt(BitConverter.ToInt32(span[8..12]));
        buffer.WriteInt(BitConverter.ToInt32(span[12..16]));
    }

    private static void WriteStringCore<TBuffer>(ref TBuffer buffer, string value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        bytes.CopyTo(buffer.AccessWrite(bytes.Length));
    }

    private static void WritePropertyCore<TBuffer>(ref TBuffer buffer, Property value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        buffer.WriteString(value.Name);
        buffer.WriteString(value.Value);
        buffer.WriteBoolean(value.IsSigned);

        if (!value.IsSigned)
            return;

        if (string.IsNullOrWhiteSpace(value.Signature))
            throw new InvalidDataException("Signature is null or whitespace, but IsSigned set to true");

        buffer.WriteString(value.Signature);
    }

    private static void WritePropertyArrayCore<TBuffer>(ref TBuffer buffer, Property[] value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        buffer.WriteVarInt(value.Length);

        foreach (var property in value)
            buffer.WriteProperty(property);
    }
}
