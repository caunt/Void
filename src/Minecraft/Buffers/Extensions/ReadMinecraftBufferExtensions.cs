using System;
using System.Buffers.Binary;
using System.Text;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Nbt;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Buffers.Extensions;

public static class ReadMinecraftBufferExtensions
{
    /// <summary>
    /// Reads a single byte from the provided buffer.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter is a reference to a buffer from which a byte will be read.</param>
    /// <returns>Returns the byte read from the buffer.</returns>
    public static byte ReadUnsignedByte<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.Read(1)[0];

    /// <summary>
    /// Reads a boolean value from the provided buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer from which the boolean value is read.</param>
    /// <returns>Returns the boolean value read from the buffer.</returns>
    public static bool ReadBoolean<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        Convert.ToBoolean(buffer.ReadUnsignedByte());

    /// <summary>
    /// Reads a variable-length short value from a buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">The reference to the buffer from which the variable-length short value is read.</param>
    /// <returns>Returns the short value read from the buffer.</returns>
    public static int ReadVarShort<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        ReadVarShortCore(ref buffer);

    /// <summary>
    /// Reads a 16-bit unsigned integer from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the unsigned short is read.</param>
    /// <returns>Returns the 16-bit unsigned integer read from the buffer.</returns>
    public static ushort ReadUnsignedShort<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadUInt16BigEndian(buffer.Read(2));

    /// <summary>
    /// Reads a 16-bit short value from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the short value is read.</param>
    /// <returns>Returns the 16-bit short value read from the buffer.</returns>
    public static short ReadShort<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadInt16BigEndian(buffer.Read(2));

    /// <summary>
    /// Reads a variable-length integer from a buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">The parameter is a reference to the buffer from which the variable-length integer is read.</param>
    /// <returns>Returns the integer value read from the buffer.</returns>
    public static int ReadVarInt<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        ReadVarIntCore(ref buffer);

    /// <summary>
    /// Reads a 32-bit integer from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type used for the buffer must be a struct that implements a specific interface.</typeparam>
    /// <param name="buffer">The buffer from which the integer is read, passed by reference.</param>
    /// <returns>The 32-bit integer read from the buffer.</returns>
    public static int ReadInt<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadInt32BigEndian(buffer.Read(4));

    /// <summary>
    /// Reads a 4-byte floating-point value from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer from which the floating-point value is read.</param>
    /// <returns>The method returns the floating-point value read from the buffer.</returns>
    public static float ReadFloat<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadSingleBigEndian(buffer.Read(4));

    /// <summary>
    /// Reads an 8-byte double value from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the double value is read.</param>
    /// <returns>The method returns the double value read from the buffer.</returns>
    public static double ReadDouble<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadDoubleBigEndian(buffer.Read(8));

    /// <summary>
    /// Reads a variable-length long value from a buffer.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">The parameter is a reference to the buffer from which the long value is read.</param>
    /// <returns>Returns the long value read from the buffer.</returns>
    public static long ReadVarLong<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        ReadVarLongCore(ref buffer);

    /// <summary>
    /// Reads a 64-bit integer from a buffer in big-endian format.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter provides access to the data from which the integer is read.</param>
    /// <returns>Returns the 64-bit integer read from the buffer.</returns>
    public static long ReadLong<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        BinaryPrimitives.ReadInt64BigEndian(buffer.Read(8));

    /// <summary>
    /// Reads a UUID from a buffer by extracting two long values.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer from which the UUID is read.</param>
    /// <returns>Returns a UUID constructed from the two long values read from the buffer.</returns>
    public static Uuid ReadUuid<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        Uuid.FromLongs(buffer.ReadLong(), buffer.ReadLong());

    /// <summary>
    /// Reads a UUID from a buffer and returns it as an integer array.
    /// </summary>
    /// <typeparam name="TBuffer">This type is a struct that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">The buffer is used to read the UUID data from its current position.</param>
    /// <returns>An integer array representing the UUID.</returns>
    public static Uuid ReadUuidAsIntArray<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        ReadUuidAsIntArrayCore(ref buffer);

    /// <summary>
    /// Reads a string from a buffer with a specified maximum length.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer from which the string is read.</param>
    /// <param name="maxLength">This parameter defines the maximum number of characters to read from the buffer.</param>
    /// <returns>The method returns the string read from the buffer.</returns>
    public static string ReadString<TBuffer>(ref this TBuffer buffer, int maxLength = 32767)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        ReadStringCore(ref buffer, maxLength);

    /// <summary>
    /// Reads a property from a buffer and returns it.
    /// </summary>
    /// <typeparam name="TBuffer">The type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The parameter is a reference to the buffer from which the property is read.</param>
    /// <returns>The method returns the property read from the buffer.</returns>
    public static Property ReadProperty<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        ReadPropertyCore(ref buffer);

    /// <summary>
    /// Reads an array of properties from a buffer, allowing for a specified number of properties to be read.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">The buffer from which properties are read, passed by reference to allow modifications.</param>
    /// <param name="count">Specifies the number of properties to read from the buffer, with a default value indicating all properties
    /// should be read.</param>
    /// <returns>An array of properties read from the buffer.</returns>
    public static Property[] ReadPropertyArray<TBuffer>(ref this TBuffer buffer, int count = -1)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        ReadPropertyArrayCore(ref buffer, count);

    /// <summary>
    /// Reads a tag from a buffer and returns it as an NbtTag object.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer from which the tag is read.</param>
    /// <returns>Returns an NbtTag object that represents the read tag.</returns>
    public static NbtTag ReadTag<TBuffer>(ref this TBuffer buffer, bool readName = false)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        NbtTag.ReadFrom(ref buffer, readName);

    /// <summary>
    /// Reads a component from a specified buffer using a given protocol version.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface for reading data.</typeparam>
    /// <param name="buffer">This parameter is the data source from which the component is read.</param>
    /// <param name="protocolVersion">This parameter indicates the version of the protocol to be used for reading the component.</param>
    /// <returns>Returns the component that has been read from the buffer.</returns>
    public static Component ReadComponent<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        Component.ReadFrom(ref buffer);

    /// <summary>
    /// Reads a byte array from the specified buffer, using a variable-length integer to determine the number of bytes.
    /// to read.
    /// </summary>
    /// <remarks>This method is useful for efficiently reading length-prefixed byte arrays from a Minecraft
    /// protocol buffer, where the length is encoded as a variable-length integer.</remarks>
    /// <typeparam name="TBuffer">The type of the buffer, which must be a value type that implements the IMinecraftBuffer<TBuffer> interface.</typeparam>
    /// <param name="buffer">A reference to the buffer from which the byte array is read. The buffer must support reading variable-length
    /// integers.</param>
    /// <returns>A read-only span of bytes containing the data read from the buffer.</returns>
    public static ReadOnlySpan<byte> ReadByteArray<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.Read(buffer.ReadVarInt());

    /// <summary>
    /// Reads all remaining data from a buffer and returns it as a read-only span of bytes.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">This parameter is a reference to the buffer from which data will be read.</param>
    /// <returns>A read-only span containing the bytes read from the buffer.</returns>
    public static ReadOnlySpan<byte> ReadToEnd<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.Read(buffer.Remaining);

    /// <summary>
    /// Debug only. Returns a read-only span of bytes from the specified buffer starting at index 0 up to its length.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">This parameter is a reference to a buffer from which the byte span is accessed.</param>
    /// <returns>The method returns a read-only span of bytes representing the contents of the buffer.</returns>
    public static ReadOnlySpan<byte> Dump<TBuffer>(ref this TBuffer buffer)
        where TBuffer : struct, IMinecraftBuffer<TBuffer>,
        allows ref struct =>
        buffer.Access(0, buffer.Length);

    /// <summary>
    /// Reads a specified number of bytes from a buffer and returns them as a read-only span.
    /// </summary>
    /// <typeparam name="TBuffer">This type parameter represents a structure that implements a specific buffer interface.</typeparam>
    /// <param name="buffer">The buffer from which bytes are read and manipulated.</param>
    /// <param name="length">The number of bytes to read from the buffer.</param>
    /// <returns>A read-only span containing the bytes that were read from the buffer.</returns>
    public static ReadOnlySpan<byte> Read<TBuffer>(ref this TBuffer buffer, int length) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var slice = buffer.Access(length);
        buffer.Seek(length);
        return slice;
    }

    private static int ReadVarShortCore<TBuffer>(ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var low = buffer.ReadUnsignedShort();
        var high = 0;

        if ((low & 0x8000) == 0)
            return (high & 0xFF) << 15 | low;

        low &= 0x7FFF;
        high = buffer.ReadUnsignedByte();

        return (high & 0xFF) << 15 | low;
    }

    private static int ReadVarIntCore<TBuffer>(ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var result = 0;

        byte read = 0;
        byte temp;

        do
        {
            temp = buffer.ReadUnsignedByte();
            var value = temp & 0b01111111;

            result |= value << 7 * read;
            read++;

            if (read > 5)
                throw new InvalidOperationException("VarInt is too big");
        }
        while ((temp & 0b10000000) != 0);

        return result;
    }

    private static long ReadVarLongCore<TBuffer>(ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        long result = 0;

        byte read = 0;
        byte temp;

        do
        {
            temp = buffer.ReadUnsignedByte();
            var value = temp & 0b01111111;

            result |= (long)value << (7 * read);
            read++;

            if (read > 10)
                throw new InvalidOperationException("VarLong is too big");
        }
        while ((temp & 0b10000000) != 0);

        return result;
    }

    private static Uuid ReadUuidAsIntArrayCore<TBuffer>(ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var msbHigh = (long)buffer.ReadInt() << 32;
        var msbLow = buffer.ReadInt() & 0xFFFFFFFFL;
        var msb = msbHigh | msbLow;

        var lsbHigh = (long)buffer.ReadInt() << 32;
        var lsbLow = buffer.ReadInt() & 0xFFFFFFFFL;
        var lsb = lsbHigh | lsbLow;

        return Uuid.FromLongs(msb, lsb);
    }

    private static string ReadStringCore<TBuffer>(ref TBuffer buffer, int maxLength = 32767) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var length = buffer.ReadVarInt();
        var span = buffer.Read(length);
        var value = Encoding.UTF8.GetString(span);

        if (maxLength > 0 && value.Length > maxLength)
            throw new IndexOutOfRangeException($"string ({value.Length}) exceeded maximum length ({maxLength})");

        return value;
    }

    private static Property ReadPropertyCore<TBuffer>(ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        var name = buffer.ReadString();
        var value = buffer.ReadString();
        var isSigned = buffer.ReadBoolean();
        var signature = isSigned ? buffer.ReadString() : null;

        return new Property(name, value, isSigned, signature);
    }

    private static Property[] ReadPropertyArrayCore<TBuffer>(ref TBuffer buffer, int count = -1) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
    {
        if (count < 0)
            count = buffer.ReadVarInt();

        var properties = new Property[count];

        for (var i = 0; i < count; i++)
            properties[i] = buffer.ReadProperty();

        return properties;
    }
}
