using System;
using System.IO;
using Void.Minecraft.Buffers.Exceptions;

namespace Void.Minecraft.Buffers;

/// <summary>
/// Manages a span of bytes, allowing access and manipulation of its position within the buffer.
/// </summary>
public ref struct BufferSpan : IMinecraftBuffer<BufferSpan>
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

    public readonly Span<byte> Access(int length)
    {
        return Access(_position, length);
    }

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
}
