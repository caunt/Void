using System;
using System.IO;
using Void.Minecraft.Buffers.Exceptions;

namespace Void.Minecraft.Buffers;

/// <summary>
/// Manages a span of bytes, allowing access and manipulation of its position within the buffer.
/// </summary>
public ref struct BufferSpan : IMinecraftBuffer<BufferSpan>, IDisposable
{
    private readonly Span<byte> _source;
    private int _position;

    public int Position
    {
        readonly get => _position;
        set
        {
            if (value < 0 || value > _source.Length)
                throw new ArgumentOutOfRangeException(nameof(value), "Position must be within the bounds of the buffer.");

            _position = value;
        }
    }

    public readonly int Length => _source.Length;
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

    public readonly BufferSpan Slice(int position, int length)
    {
        return new BufferSpan(Access(position, length));
    }

    /// <summary>
    /// Returns a writable view of <paramref name="length"/> bytes starting at the current <see cref="Position"/>.
    /// </summary>
    /// <remarks>
    /// <para>This method intentionally does not advance <see cref="Position"/>; callers that consume or fill the returned span must move the cursor explicitly, for example with <see cref="Seek(int, SeekOrigin)"/>.</para>
    /// <para>The returned span aliases the underlying buffer, so writing through it mutates the same storage owned by this <see cref="BufferSpan"/> instance.</para>
    /// </remarks>
    /// <param name="length">The number of bytes to expose from the current position.</param>
    /// <returns>A writable <see cref="Span{T}"/> over the requested region. When <paramref name="length"/> is <c>0</c>, an empty span at the current <see cref="Position"/> is returned.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is negative.</exception>
    /// <exception cref="EndOfBufferException">Thrown when the requested range extends past the end of the underlying span.</exception>
    /// <example>
    /// <code>
    /// var buffer = new BufferSpan(stackalloc byte[8]);
    /// var payload = buffer.Access(4);
    /// payload[0] = 0x01;
    /// buffer.Seek(4);
    /// </code>
    /// </example>
    /// <seealso cref="Access(int, int)"/>
    /// <seealso cref="Seek(int, SeekOrigin)"/>
    public readonly Span<byte> Access(int length)
    {
        return Access(_position, length);
    }

    /// <summary>
    /// Returns a writable view of <paramref name="length"/> bytes starting at an absolute <paramref name="position"/>.
    /// </summary>
    /// <remarks>
    /// <para>This overload performs bounds validation and then delegates to <see cref="Span{T}.Slice(int, int)"/> on the underlying storage.</para>
    /// <para>It does not modify <see cref="Position"/>, which allows random access reads or writes without changing the current cursor.</para>
    /// </remarks>
    /// <param name="position">The zero-based index in the underlying span where the returned region begins.</param>
    /// <param name="length">The number of bytes in the returned region.</param>
    /// <returns>A writable <see cref="Span{T}"/> over the specified region. When <paramref name="length"/> is <c>0</c>, an empty span at <paramref name="position"/> is returned.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="position"/> or <paramref name="length"/> is negative.</exception>
    /// <exception cref="EndOfBufferException">Thrown when <paramref name="position"/> + <paramref name="length"/> exceeds <see cref="Length"/>.</exception>
    /// <example>
    /// <code>
    /// var buffer = new BufferSpan(stackalloc byte[16]);
    /// var header = buffer.Access(0, 2);
    /// header[0] = 0xAA;
    /// header[1] = 0xBB;
    /// </code>
    /// </example>
    /// <seealso cref="Access(int)"/>
    /// <seealso cref="Position"/>
    public readonly Span<byte> Access(int position, int length)
    {
        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(position), "Value must be non-negative.");

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Value must be non-negative.");

        if (position + length > _source.Length)
            throw new EndOfBufferException(_source.Length, position, length);

        return _source.Slice(position, length);
    }

    /// <summary>
    /// Moves the current position within the underlying span relative to the specified origin.
    /// </summary>
    /// <param name="offset">
    /// The byte offset applied from <paramref name="origin"/>.
    /// </param>
    /// <param name="origin">
    /// The reference point used to compute the new position.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="origin"/> is not a supported <see cref="SeekOrigin"/> value.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the resulting position would be negative.
    /// </exception>
    /// <exception cref="EndOfBufferException">
    /// Thrown when the resulting position would move past the end of the span.
    /// </exception>
    public void Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
    {
        var position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => _source.Length + offset,
            _ => throw new ArgumentException($"Invalid SeekOrigin value: {origin}. Expected Begin, Current, or End.", nameof(origin)),
        };

        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "New position must be non-negative.");

        if (position > _source.Length)
            throw new EndOfBufferException(_source.Length, _position, _position - position);

        _position = position;
    }

    public readonly void Dispose()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Remaining, nameof(Remaining));

        if (Remaining is 0)
            return;

        if (Remaining > 0)
            throw new BufferRemainingDataException(_source.Length, _position);
    }
}
