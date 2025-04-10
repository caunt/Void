using System;
using System.Buffers.Binary;
using System.IO;
using Void.Minecraft.Buffers.Exceptions;

namespace Void.Minecraft.Buffers;

/// <summary>
/// Manages a span of bytes, allowing reading and writing operations while tracking the current position. It ensures
/// operations stay within buffer bounds.
/// </summary>
public ref struct BufferSpan
{
    private readonly Span<byte> _source;
    private int _position;

    /// <summary>
    /// Gets or sets the position within a buffer. The position must be between 0 and the length of the source,
    /// otherwise an exception is thrown.
    /// </summary>
    public int Position { readonly get => _position; set => _position = (value >= 0 && value <= _source.Length) ? value : throw new ArgumentOutOfRangeException(nameof(value), "Position must be within the bounds of the buffer."); }

    /// <summary>
    /// Returns the length of the source as an integer. It is a read-only property.
    /// </summary>
    public readonly int Length => _source.Length;

    /// <summary>
    /// Calculates the number of remaining elements in a source collection. It subtracts the current position from the
    /// total length.
    /// </summary>
    public readonly int Remaining => _source.Length - _position;

    /// <summary>
    /// Initializes a new instance of the BufferSpan class with a byte span.
    /// </summary>
    /// <param name="source">The byte span provides the data to be managed within the BufferSpan instance.</param>
    public BufferSpan(Span<byte> source)
    {
        _source = source;
        _position = 0;
    }

    /// <summary>
    /// Reads a single byte from the source at the current position and advances the position.
    /// </summary>
    /// <returns>Returns the byte read from the source.</returns>
    /// <exception cref="EndOfBufferException">Thrown when attempting to read beyond the end of the source buffer.</exception>
    public byte ReadByte()
    {
        if (_position >= _source.Length)
            throw new EndOfBufferException(_position, 1, _source.Length);

        return _source[_position++];
    }

    /// <summary>
    /// Writes a byte value to a specified position in a buffer. It increments the position after writing the value.
    /// </summary>
    /// <param name="value">The byte to be written to the buffer at the current position.</param>
    /// <exception cref="EndOfBufferException">Thrown when attempting to write beyond the end of the buffer.</exception>
    public void WriteByte(byte value)
    {
        if (_position >= _source.Length)
            throw new EndOfBufferException(_position, 1, _source.Length);

        _source[_position++] = value;
    }

    /// <summary>
    /// Reads two bytes from a stream and interprets them as an unsigned short in big-endian format.
    /// </summary>
    /// <returns>Returns the unsigned short value read from the stream.</returns>
    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(ReadSource(2));
    }

    /// <summary>
    /// Writes a 16-bit unsigned integer in big-endian format to a binary source.
    /// </summary>
    /// <param name="value">The number to be written as a 16-bit unsigned integer.</param>
    public void WriteUnsignedShort(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(ReadSource(2), value);
    }

    /// <summary>
    /// Reads a 16-bit short value from a binary source using big-endian byte order. It retrieves 2 bytes from the
    /// source and converts them to a short.
    /// </summary>
    /// <returns>Returns the short value read from the binary source.</returns>
    public short ReadShort()
    {
        return BinaryPrimitives.ReadInt16BigEndian(ReadSource(2));
    }

    /// <summary>
    /// Writes a 16-bit integer in big-endian format to a specified source.
    /// </summary>
    /// <param name="value">The integer to be written, represented as a 16-bit short.</param>
    public void WriteShort(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(ReadSource(2), value);
    }

    /// <summary>
    /// Reads a 4-byte integer from a binary source in big-endian format. It uses a method to read the bytes and then
    /// converts them to an integer.
    /// </summary>
    /// <returns>Returns the integer value read from the binary source.</returns>
    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(ReadSource(4));
    }

    /// <summary>
    /// Writes a 32-bit integer in big-endian format to a specified source.
    /// </summary>
    /// <param name="value">The integer to be written in big-endian format.</param>
    public void WriteInt(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(ReadSource(4), value);
    }

    /// <summary>
    /// Reads a 4-byte floating-point number from a binary source in big-endian format. It utilizes the ReadSource
    /// method to obtain the byte data.
    /// </summary>
    /// <returns>Returns the floating-point number read from the binary source.</returns>
    public float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(ReadSource(4));
    }

    /// <summary>
    /// Writes a floating-point number in big-endian format to a binary source.
    /// </summary>
    /// <param name="value">The floating-point number to be written in big-endian format.</param>
    public void WriteFloat(float value)
    {
        BinaryPrimitives.WriteSingleBigEndian(ReadSource(4), value);
    }

    /// <summary>
    /// Reads an 8-byte double value from a binary source in big-endian format. It utilizes a method to read the source
    /// data before converting it.
    /// </summary>
    /// <returns>Returns the double value read from the binary source.</returns>
    public double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleBigEndian(ReadSource(8));
    }

    /// <summary>
    /// Writes a double value to a binary source in big-endian format.
    /// </summary>
    /// <param name="value">The double value to be written in a specific binary format.</param>
    public void WriteDouble(double value)
    {
        BinaryPrimitives.WriteDoubleBigEndian(ReadSource(8), value);
    }

    /// <summary>
    /// Reads a 64-bit long integer from a binary source using big-endian format. It retrieves 8 bytes from the source
    /// for the conversion.
    /// </summary>
    /// <returns>Returns the 64-bit long integer read from the binary source.</returns>
    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(ReadSource(8));
    }

    /// <summary>
    /// Writes a 64-bit integer in big-endian format to a specified source.
    /// </summary>
    /// <param name="value">The long integer to be written in big-endian format.</param>
    public void WriteLong(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(ReadSource(8), value);
    }

    /// <summary>
    /// Advances the current position in the buffer by the specified offset.
    /// </summary>
    /// <param name="offset">The number of bytes to move forward.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the offset is negative.
    /// </exception>
    /// <exception cref="EndOfBufferException">
    /// Thrown when the new position would exceed the buffer's length.
    /// </exception>
    public void Seek(int offset)
    {
        Seek(offset, SeekOrigin.Current);
    }

    /// <summary>
    /// Advances or sets the current position in the buffer relative to the specified origin.
    /// </summary>
    /// <param name="offset">The byte offset relative to the <paramref name="origin"/>.</param>
    /// <param name="origin">The point of reference used to obtain the new position.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the calculated new position is negative.
    /// </exception>
    /// <exception cref="EndOfBufferException">
    /// Thrown when the calculated new position exceeds the bounds of the buffer.
    /// </exception>
    public void Seek(int offset, SeekOrigin origin)
    {
        var position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => _source.Length + offset,
            _ => throw new ArgumentException("Invalid SeekOrigin", nameof(origin)),
        };
        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "New position must be non-negative.");

        if (position > _source.Length)
            throw new EndOfBufferException(_position, _position - position, _source.Length);

        _position = position;
    }

    /// <summary>
    /// Creates a new BufferSpan from a specified starting position and length within the source buffer.
    /// </summary>
    /// <param name="position">Indicates the starting point in the source buffer for slicing.</param>
    /// <param name="length">Specifies the number of elements to include in the slice from the starting position.</param>
    /// <returns>Returns a new BufferSpan that represents the sliced portion of the source buffer.</returns>
    public readonly BufferSpan Slice(int position, int length)
    {
        return new BufferSpan(SliceSource(position, length));
    }

    /// <summary>
    /// Reads the specified number of bytes starting from the current position 
    /// and advances the position by that length.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}"/> representing the bytes read from the current position.
    /// </returns>
    /// <exception cref="EndOfBufferException">
    /// Thrown when there aren't enough bytes remaining in the buffer.
    /// </exception>
    public ReadOnlySpan<byte> Read(int length)
    {
        return ReadSource(length);
    }

    /// <summary>
    /// Writes the provided byte data to a destination.
    /// </summary>
    /// <param name="value">The byte data to be written, represented as a read-only span.</param>
    public void Write(ReadOnlySpan<byte> value)
    {
        value.CopyTo(ReadSource(value.Length));
    }

    /// <summary>
    /// Retrieves a slice of the underlying buffer of the specified length starting at the current position,
    /// and advances the current position by that length.
    /// </summary>
    /// <param name="length">The number of bytes to read from the current position.</param>
    /// <returns>
    /// A <see cref="Span{T}"/> containing the slice of the underlying buffer starting at the original position
    /// and extending for the specified length.
    /// </returns>
    private Span<byte> ReadSource(int length)
    {
        var slice = SliceSource(length);
        _position += length;

        return slice;
    }

    /// <summary>
    /// Returns a writable slice of the underlying buffer with the specified length,
    /// starting at the current position, without advancing the position.
    /// </summary>
    /// <param name="length">The number of bytes to include in the slice.</param>
    /// <returns>
    /// A <see cref="Span{T}"/> representing the slice of the underlying buffer starting at the current position.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the requested slice exceeds the bounds of the underlying buffer.
    /// </exception>
    private readonly Span<byte> SliceSource(int length)
    {
        return SliceSource(_position, length);
    }

    /// <summary>
    /// Extracts a slice from a byte span based on specified position and length.
    /// </summary>
    /// <param name="position">Indicates the starting index for the slice within the source span.</param>
    /// <param name="length">Specifies the number of bytes to include in the slice from the starting index.</param>
    /// <returns>Returns a new span containing the specified slice of bytes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the starting index or length is negative.</exception>
    /// <exception cref="EndOfBufferException">Thrown when the sum of the starting index and length exceeds the source span's length.</exception>
    private readonly Span<byte> SliceSource(int position, int length)
    {
        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(position), "Value must be non-negative.");

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Value must be non-negative.");

        if (position + length > _source.Length)
            throw new EndOfBufferException(position, length, _source.Length);

        return _source.Slice(position, length);
    }
}
