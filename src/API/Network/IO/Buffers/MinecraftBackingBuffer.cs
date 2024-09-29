using System.Buffers;
using System.Data;
using System.Text;
using Void.Proxy.API.Mojang;
using Void.Proxy.API.Mojang.Profiles;
using Void.Proxy.API.Network.IO.Buffers.ReadOnly;
using Void.Proxy.API.Network.IO.Buffers.ReadWrite;

namespace Void.Proxy.API.Network.IO.Buffers;

internal ref struct MinecraftBackingBuffer
{
    private SpanBackingBuffer _spanBackingBuffer;
    private ReadOnlySpanBackingBuffer _readOnlySpanBackingBuffer;
    private ReadOnlySequenceBackingBuffer _readOnlySequenceBackingBuffer;
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

    public bool HasData()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Position < _spanBackingBuffer.Length,
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Position < _readOnlySpanBackingBuffer.Length,
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Position < _readOnlySequenceBackingBuffer.Length,
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public int GetPosition()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Position,
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Position,
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Position,
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
    }

    public int GetLength()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Length,
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Length,
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Length,
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
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public int ReadVarShort()
    {
        var low = ReadUnsignedShort();
        var high = 0;

        if ((low & 0x8000) == 0)
            return ((high & 0xFF) << 15) | low;

        low &= 0x7FFF;
        high = ReadUnsignedByte();

        return ((high & 0xFF) << 15) | low;
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
        var result = 0;
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
        foreach (var temp in MinecraftBuffer.EnumerateVarInt(value))
            WriteUnsignedByte(temp);
    }

    public int ReadInt()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadInt(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadInt(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadInt(),
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
        var bytes = value.AsGuid.ToByteArray();
        var msb = BitConverter.ToUInt64(bytes, 0);
        var lsb = BitConverter.ToUInt64(bytes, 8);

        WriteInt((int)(msb >> 32));
        WriteInt((int)msb);
        WriteInt((int)(lsb >> 32));
        WriteInt((int)lsb);
    }

    public string ReadString(int maxLength = 32767)
    {
        var length = ReadVarInt();
        var span = _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Read(length),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Read(length),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Read(length),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };

        var value = Encoding.UTF8.GetString(span);

        if (maxLength > 0 && value.Length > maxLength)
            throw new IndexOutOfRangeException($"string ({value.Length}) exceeded maximum length ({maxLength})");

        return value;
    }

    public void WriteString(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        WriteVarInt(length);

        var span = _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Slice(length),
            BufferType.ReadOnlySpan or BufferType.ReadOnlySequence => throw new ReadOnlyException(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };

        Encoding.UTF8.GetBytes(value, span);
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

    public void Seek(int offset, SeekOrigin origin)
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
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public ReadOnlySpan<byte> Read(int length)
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.Read(length),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.Read(length),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.Read(length),
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
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    public ReadOnlySpan<byte> ReadToEnd()
    {
        return _bufferType switch
        {
            BufferType.Span => _spanBackingBuffer.ReadToEnd(),
            BufferType.ReadOnlySpan => _readOnlySpanBackingBuffer.ReadToEnd(),
            BufferType.ReadOnlySequence => _readOnlySequenceBackingBuffer.ReadToEnd(),
            _ => throw new NotSupportedException(_bufferType.ToString())
        };
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
            default:
                throw new NotSupportedException(_bufferType.ToString());
        }
    }

    private enum BufferType
    {
        Span,
        ReadOnlySpan,
        ReadOnlySequence
    }
}