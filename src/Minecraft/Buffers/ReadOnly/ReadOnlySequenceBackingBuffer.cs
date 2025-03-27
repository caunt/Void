using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;

namespace Void.Minecraft.Buffers.ReadOnly;

internal ref struct ReadOnlySequenceBackingBuffer
{
    private readonly ReadOnlySequence<byte> _sequence;
    private ReadOnlySequence<byte>.Enumerator _enumerator;
    private ReadOnlySpan<byte> _currentBlock;
    private long _blockPosition;

    public long Position;
    public long Length { get; init; }

    public ReadOnlySequenceBackingBuffer(ReadOnlySequence<byte> sequence)
    {
        Position = 0;
        Length = sequence.Length;

        _sequence = sequence;
        _enumerator = sequence.GetEnumerator();
        MoveNextBlock();
    }

    public byte ReadUnsignedByte()
    {
        if (_blockPosition >= _currentBlock.Length)
            MoveNextBlock();

        Position++;
        return _currentBlock[(int)_blockPosition++];
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(Read(2));
    }

    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(Read(4));
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(Read(8));
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        var targetPosition = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => Length - offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), "Invalid seek origin")
        };

        if (targetPosition < 0 || targetPosition > _sequence.Length)
            throw new ArgumentOutOfRangeException(nameof(offset), "Attempted to seek outside the bounds of the sequence");

        Reset();

        while (targetPosition > Position)
        {
            if (Position + _currentBlock.Length >= targetPosition)
            {
                _blockPosition = targetPosition - Position;
                Position = targetPosition;
                break;
            }

            Position += _currentBlock.Length;
            MoveNextBlock();
        }
    }

    public void Reset()
    {
        _enumerator = _sequence.GetEnumerator();
        Position = 0;
        MoveNextBlock();
    }

    public ReadOnlySpan<byte> Slice(long length)
    {
        if (_sequence.Length < Position + length)
            throw new IndexOutOfRangeException($"Cannot slice {length} bytes from sequence with length {_sequence.Length}, and current position {Position}. Only {_sequence.Length - Position} bytes is available to slice.");

        if (_currentBlock.Length < _blockPosition + length)
        {
            // very bad case (for example, protocol message is split across multiple segments)
            // TODO if possible, rework this to avoid allocation as they are large here
            // previous code:
            // throw new IndexOutOfRangeException($"Current block length is {_currentBlock.Length} and position is {_blockPosition}, attempted to slice {length} bytes, but reading from next blocks not supported. Sequence length is {_sequence.Length}.");
            var sequence = _sequence.Slice(Position, length);
            Position += length;
            Seek(Position, SeekOrigin.Begin);

            Console.WriteLine($"Allocated {sequence.Length:N0} bytes on heap when reading not contiguous blocks of memory");
            return sequence.ToArray();
        }
        else
        {
            // goes here in most cases
            var span = _currentBlock.Slice((int)_blockPosition, (int)length);

            _blockPosition += length;
            Position += length;

            return span;
        }
    }

    public ReadOnlySpan<byte> Read(long length)
    {
        return Slice(length);
    }

    // Read Slice(int length) for explanation
    // TODO: is it now possible? review please
    // may be we are in last block and we can read it to the end
    // public ReadOnlySpan<byte> ReadToEnd()
    // {
    //     var sequencePosition = _sequence.GetPosition(Position);
    //     Position = Length;
    //     return _sequence.Slice(sequencePosition);
    // }

    private void MoveNextBlock()
    {
        if (!_enumerator.MoveNext())
            throw new IndexOutOfRangeException();

        _blockPosition = 0;
        _currentBlock = _enumerator.Current.Span;
    }
}