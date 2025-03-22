using Serilog;
using System.Buffers;
using System.Buffers.Binary;

namespace Void.Proxy.API.Network.IO.Buffers.ReadOnly;

internal ref struct ReadOnlySequenceBackingBuffer
{
    private readonly ReadOnlySequence<byte> _sequence;
    private ReadOnlySequence<byte>.Enumerator _enumerator;
    private ReadOnlySpan<byte> _currentBlock;
    private int _blockPosition;

    public int Position;
    public int Length { get; init; }

    public ReadOnlySequenceBackingBuffer(ReadOnlySequence<byte> sequence)
    {
        Position = 0;
        Length = (int)sequence.Length;

        _sequence = sequence;
        _enumerator = sequence.GetEnumerator();
        MoveNextBlock();
    }

    public byte ReadUnsignedByte()
    {
        if (_blockPosition >= _currentBlock.Length)
            MoveNextBlock();

        Position++;
        return _currentBlock[_blockPosition++];
    }

    public ushort ReadUnsignedShort()
    {
        if (_blockPosition + 2 <= _currentBlock.Length)
        {
            // Attempt to read the unsigned short from the current block
            var result = BinaryPrimitives.ReadUInt16BigEndian(_currentBlock.Slice(_blockPosition, 2));

            _blockPosition += 2;
            Position += 2;

            return result;
        }
        else
        {
            // If the unsigned short is split across blocks, read byte by byte
            ushort result = 0;

            for (var i = 0; i < 2; i++)
                result |= (ushort)(ReadUnsignedByte() << (8 * i));

            return result;
        }
    }

    public int ReadInt()
    {
        if (_blockPosition + 4 <= _currentBlock.Length)
        {
            // Attempt to read the int from the current block
            var result = BinaryPrimitives.ReadInt32BigEndian(_currentBlock.Slice(_blockPosition, 4));

            _blockPosition += 4;
            Position += 4;

            return result;
        }
        else
        {
            // If the int is split across blocks, read byte by byte
            var result = 0;

            for (var i = 0; i < 4; i++)
                result |= ReadUnsignedByte() << (8 * i); // Shifting by 8 bits per byte

            return result;
        }
    }

    public long ReadLong()
    {
        if (_blockPosition + 8 <= _currentBlock.Length)
        {
            // Attempt to read the long from the current block
            var result = BinaryPrimitives.ReadInt64BigEndian(_currentBlock.Slice(_blockPosition, 8));

            _blockPosition += 8;
            Position += 8;

            return result;
        }
        else
        {
            // If the long is split across blocks, read byte by byte
            var result = 0L;

            for (var i = 0; i < 8; i++)
                result |= (long)ReadUnsignedByte() << (8 * i);

            return result;
        }
    }

    // not sure if works properly
    public void Seek(int offset, SeekOrigin origin)
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

        // Reset enumerator and blocks
        Reset();

        // Move to the target position
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

    public ReadOnlySpan<byte> Slice(int length)
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

            Log.ForContext("SourceContext", nameof(ReadOnlySequenceBackingBuffer)).Warning("Allocated {Size:N0} bytes on heap when reading not contiguous blocks of memory", sequence.Length);
            return sequence.ToArray();
        }
        else
        {
            // goes here in most cases
            var span = _currentBlock.Slice(_blockPosition, length);

            _blockPosition += length;
            Position += length;

            return span;
        }
    }

    public ReadOnlySpan<byte> Read(int length)
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