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
    /// <summary>
    /// Gets how many bytes can still be read or written from the current <see cref="Position"/> to the end of the span.
    /// </summary>
    /// <value>
    /// The result of <c><see cref="Length"/> - <see cref="Position"/></c>.
    /// </value>
    /// <remarks>
    /// <para>
    /// Extension methods such as <see cref="Extensions.ReadMinecraftBufferExtensions.ReadToEnd{TBuffer}(ref TBuffer)"/> use this value to consume unread bytes.
    /// </para>
    /// <para>
    /// <see cref="Dispose"/> uses this property as a completeness guard and throws <see cref="BufferRemainingDataException"/> when the value is greater than <c>0</c>.
    /// </para>
    /// </remarks>
    /// <seealso cref="Length"/>
    /// <seealso cref="Position"/>
    /// <seealso cref="Dispose"/>
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
    /// Creates a new <see cref="BufferSpan"/> over a sub-range of the current underlying storage.
    /// </summary>
    /// <remarks>
    /// <para>The returned <see cref="BufferSpan"/> points to the same backing memory as this instance but is constrained to the requested region.</para>
    /// <para>The new instance starts with <see cref="Position"/> set to <c>0</c> relative to the sliced region, while this instance's <see cref="Position"/> is not modified.</para>
    /// </remarks>
    /// <param name="position">The zero-based offset in the current underlying span where the slice begins.</param>
    /// <param name="length">The number of bytes included in the slice.</param>
    /// <returns>A new <see cref="BufferSpan"/> representing <paramref name="length"/> bytes starting at <paramref name="position"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="position"/> or <paramref name="length"/> is negative.</exception>
    /// <exception cref="EndOfBufferException">Thrown when <paramref name="position"/> + <paramref name="length"/> exceeds <see cref="Length"/>.</exception>
    /// <example>
    /// <code>
    /// var source = new BufferSpan(stackalloc byte[8]);
    /// var header = source.Slice(0, 2);
    /// header.WriteUnsignedByte(0xAA);
    /// </code>
    /// </example>
    /// <seealso cref="Access(int, int)"/>
    /// <seealso cref="Position"/>
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

    /// <summary>
    /// Validates that all bytes in this span were consumed before ending its usage scope.
    /// </summary>
    /// <remarks>
    /// <para>This method is used as a guard: it does not release unmanaged resources.</para>
    /// <para>When bytes remain unread or unwritten (<see cref="Remaining"/> &gt; <c>0</c>), it throws <see cref="BufferRemainingDataException"/> so callers can detect incomplete protocol parsing/serialization.</para>
    /// <para>When <see cref="Remaining"/> is <c>0</c>, it returns normally.</para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="Remaining"/> is negative, which indicates an invalid cursor state.</exception>
    /// <exception cref="BufferRemainingDataException">Thrown when there is unconsumed data left in the span.</exception>
    /// <example>
    /// <code>
    /// var buffer = new BufferSpan(stackalloc byte[4]);
    /// buffer.WriteInt(42);
    /// buffer.Seek(0, SeekOrigin.Begin);
    /// _ = buffer.ReadInt();
    /// buffer.Dispose();
    /// </code>
    /// </example>
    /// <seealso cref="Remaining"/>
    /// <seealso cref="Seek(int, SeekOrigin)"/>
    public readonly void Dispose()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Remaining, nameof(Remaining));

        if (Remaining is 0)
            return;

        if (Remaining > 0)
            throw new BufferRemainingDataException(_source.Length, _position);
    }
}
