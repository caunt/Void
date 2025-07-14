using System;
using System.Buffers;
using System.Data;
using System.IO;
using System.Buffers.Binary;
using System.Text;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Buffers.ReadOnly;
using Void.Minecraft.Buffers.ReadWrite;
using Void.Minecraft.Nbt;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Buffers;

internal ref struct MinecraftBackingBuffer
{
    private SpanBackingBuffer _spanBackingBuffer;
    private ReadOnlySpanBackingBuffer _readOnlySpanBackingBuffer;
    private ReadOnlySequenceBackingBuffer _readOnlySequenceBackingBuffer;
    private MemoryStreamBackingBuffer _memoryStreamBackingBuffer;
    private readonly BufferType _bufferType;

    public MinecraftBackingBuffer(Span<byte> span)
    {
        _spanBackingBuffer = new SpanBackingBuffer(span);
        _bufferType = BufferType.Span;
    }

    public MinecraftBackingBuffer(ReadOnlySpan<byte> span)
    {
        _readOnlySpanBackingBuffer = new ReadOnlySpanBackingBuffer(span);
        _bufferType = BufferType.ReadOnlySpan;
    }

    public MinecraftBackingBuffer(ReadOnlySequence<byte> span)
    {
        _readOnlySequenceBackingBuffer = new ReadOnlySequenceBackingBuffer(span);
        _bufferType = BufferType.ReadOnlySequence;
    }

    public MinecraftBackingBuffer(MemoryStream memoryStream)
    {
        _memoryStreamBackingBuffer = new MemoryStreamBackingBuffer(memoryStream);
        _bufferType = BufferType.MemoryStream;
    }

    public readonly bool HasData()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Position < _spanBackingBuffer.Length,
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Position < _readOnlySpanBackingBuffer.Length,
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Position < _readOnlySequenceBackingBuffer.Length,
            BufferType.MemoryStream => _memoryStreamBackingBuffer.Position < _readOnlySequenceBackingBuffer.Length,
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public readonly long GetPosition()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Position,
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Position,
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Position,
            BufferType.MemoryStream => _memoryStreamBackingBuffer.Position,
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public readonly long GetLength()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Length,
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Length,
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Length,
            BufferType.MemoryStream => _memoryStreamBackingBuffer.Length,
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public byte ReadUnsignedByte()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadUnsignedByte(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadUnsignedByte(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadUnsignedByte(),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.ReadUnsignedByte(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void WriteUnsignedByte(byte value)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.WriteUnsignedByte(value);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.WriteUnsignedByte(value);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public bool ReadBoolean()
    {
        return Convert.ToBoolean(ReadUnsignedByte());
    }

    public void WriteBoolean(bool value)
    {
        WriteUnsignedByte(Convert.ToByte(value));
    }

    public ushort ReadUnsignedShort()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadUnsignedShort(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadUnsignedShort(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadUnsignedShort(),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.ReadUnsignedShort(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void WriteUnsignedShort(ushort value)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.WriteUnsignedShort(value);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.WriteUnsignedShort(value);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public short ReadShort()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadShort(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadShort(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadShort(),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.ReadShort(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void WriteShort(short value)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.WriteShort(value);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.WriteShort(value);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public int ReadVarShort()
    {
        var low = ReadUnsignedShort();
        var high = 0;

        if ((low & 0x8000) == 0)
            return (high & 0xFF) << 15 | low;

        low &= 0x7FFF;
        high = ReadUnsignedByte();

        return (high & 0xFF) << 15 | low;
    }

    public void WriteVarShort(int value)
    {
        var low = (ushort)(value & 0x7FFF);
        var high = (byte)((value & 0x7F8000) >> 15);

        if (high != 0)
            low |= 0x8000;

        WriteUnsignedShort(low);

        if (high != 0)
            WriteUnsignedByte(high);
    }

    public int ReadVarInt()
    {
        int result = 0;
        byte read = 0;

        byte buffer;
        do
        {
            buffer = ReadUnsignedByte();
            var value = buffer & 0b01111111;
            result |= value << (7 * read);

            read++;
            if (read > 5)
                throw new InvalidOperationException("VarInt is too big");
        } while ((buffer & 0b10000000) != 0);

        return result;
    }

    public void WriteVarInt(int value)
    {
        Span<byte> buffer = stackalloc byte[5];
        var length = value.AsVarInt(buffer);
        Write(buffer[..length].ToArray());
    }

    public long ReadVarLong()
    {
        long result = 0;
        byte read = 0;

        byte buffer;
        do
        {
            buffer = ReadUnsignedByte();
            var value = buffer & 0b01111111;
            result |= (long)value << (7 * read);

            read++;
            if (read > 10)
                throw new InvalidOperationException("VarLong is too big");
        } while ((buffer & 0b10000000) != 0);

        return result;
    }

    public void WriteVarLong(long value)
    {
        while (true)
        {
            if ((value & ~(long)0x7F) == 0)
            {
                WriteUnsignedByte((byte)value);
                return;
            }

            WriteUnsignedByte((byte)((value & 0x7F) | 0x80));

            value >>>= 7;
        }
    }

    public int ReadInt()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadInt(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadInt(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadInt(),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.ReadInt(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void WriteInt(int value)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.WriteInt(value);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.WriteInt(value);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public float ReadFloat()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadFloat(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadFloat(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadFloat(),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.ReadFloat(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void WriteFloat(float value)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.WriteFloat(value);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.WriteFloat(value);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public double ReadDouble()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadDouble(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadDouble(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadDouble(),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.ReadDouble(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void WriteDouble(double value)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.WriteDouble(value);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.WriteDouble(value);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public long ReadLong()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadLong(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadLong(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadLong(),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.ReadLong(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void WriteLong(long value)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.WriteLong(value);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.WriteLong(value);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public Uuid ReadUuid()
    {
        return Uuid.FromLongs(ReadLong(), ReadLong());
    }

    public void WriteUuid(Uuid value)
    {
        Write(value.AsGuid == Guid.Empty ? new byte[16] : value.AsGuid.ToByteArray(true));
    }

    public Uuid ReadUuidAsIntArray()
    {
        var msbHigh = (long)ReadInt() << 32;
        var msbLow = ReadInt() & 0xFFFFFFFFL;
        var msb = msbHigh | msbLow;
        var lsbHigh = (long)ReadInt() << 32;
        var lsbLow = ReadInt() & 0xFFFFFFFFL;
        var lsb = lsbHigh | lsbLow;

        return Uuid.FromLongs(msb, lsb);
    }

    public void WriteUuidAsIntArray(Uuid value)
    {
        var span = value.AsGuid.ToByteArray(true).AsSpan();

        WriteInt(BinaryPrimitives.ReadInt32BigEndian(span[..4]));
        WriteInt(BinaryPrimitives.ReadInt32BigEndian(span[4..8]));
        WriteInt(BinaryPrimitives.ReadInt32BigEndian(span[8..12]));
        WriteInt(BinaryPrimitives.ReadInt32BigEndian(span[12..16]));
    }

    public string ReadString(int maxLength = 32767)
    {
        var length = ReadVarInt();
        var span = Read(length);

        var value = Encoding.UTF8.GetString(span);

        if (maxLength > 0 && value.Length > maxLength)
            throw new IndexOutOfRangeException($"string ({value.Length}) exceeded maximum length ({maxLength})");

        return value;
    }

    public void WriteString(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        WriteVarInt(length);

        switch (_bufferType)
        {
            case BufferType.Span:
                var span = _spanBackingBuffer.Slice(length);
                Encoding.UTF8.GetBytes(value, span);
                break;
            case BufferType.ReadOnlySpan or BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.Write(Encoding.UTF8.GetBytes(value));
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public Property ReadProperty()
    {
        var name = ReadString();
        var value = ReadString();
        var isSigned = ReadBoolean();
        var signature = isSigned ? ReadString() : null;

        return new Property(name, value, isSigned, signature);
    }

    public void WriteProperty(Property value)
    {
        WriteString(value.Name);
        WriteString(value.Value);
        WriteBoolean(value.IsSigned);

        if (!value.IsSigned)
            return;

        if (string.IsNullOrWhiteSpace(value.Signature))
            throw new InvalidDataException("Signature is null or whitespace, but IsSigned set to true");

        WriteString(value.Signature);
    }

    public Property[] ReadPropertyArray(int count = -1)
    {
        if (count < 0)
            count = ReadVarInt();

        var array = new Property[count];

        for (var i = 0; i < count; i++)
            array[i] = ReadProperty();

        return array;
    }

    public void WritePropertyArray(Property[] value)
    {
        WriteVarInt(value.Length);

        foreach (var property in value)
            WriteProperty(property);
    }

    public NbtTag ReadTag(bool readName = false)
    {
        var position = GetPosition();
        var data = ReadToEnd();

        // TODO another one allocation to be removed
        var length = NbtTag.Parse(data.ToArray(), out var nbt, readName);
        Seek(position + length, SeekOrigin.Begin);

        return nbt;
    }

    public void WriteTag(NbtTag value)
    {
        Write(value.AsStream());
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.Seek(offset, origin);
                break;
            case BufferType.ReadOnlySpan:
                _readOnlySpanBackingBuffer.Seek(offset, origin);
                break;
            case BufferType.ReadOnlySequence:
                _readOnlySequenceBackingBuffer.Seek(offset, origin);
                break;
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.Seek(offset, origin);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public ReadOnlySpan<byte> Read(long length)
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Read(length),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Read(length),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Read(length),
            BufferType.MemoryStream => _memoryStreamBackingBuffer.Read(length),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.Write(data);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.Write(data);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public void Write(Stream stream)
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.Write(stream);
                break;
            case BufferType.ReadOnlySpan:
            case BufferType.ReadOnlySequence:
                throw new ReadOnlyException();
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.Write(stream);
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public ReadOnlySpan<byte> ReadToEnd()
    {
        return Read(GetLength() - GetPosition());
    }

    public void Reset()
    {
        switch (_bufferType)
        {
            case BufferType.Span:
                _spanBackingBuffer.Reset();
                break;
            case BufferType.ReadOnlySpan:
                _readOnlySpanBackingBuffer.Reset();
                break;
            case BufferType.ReadOnlySequence:
                _readOnlySequenceBackingBuffer.Reset();
                break;
            case BufferType.MemoryStream:
                _memoryStreamBackingBuffer.Reset();
                break;
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    private enum BufferType
    {
        Span,
        ReadOnlySpan,
        ReadOnlySequence,
        MemoryStream
    }
}
