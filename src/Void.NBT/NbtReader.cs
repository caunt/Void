using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Void.NBT.Tags;

namespace Void.NBT
{
    public class NbtReader
    {
        private readonly byte[] _data;

        public NbtReader(Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);

            _data = ms.ToArray();
        }

        public NbtReader(byte[] data)
        {
            _data = data;
        }

        public Encoding StringEncoder { get; set; } = Encoding.UTF8;
        public int Position { get; private set; }

        public void Skip(int length)
        {
            Position += length;
        }

        public NbtTagType ReadTagType()
        {
            return (NbtTagType)ReadByte();
        }

        public NbtTag ReadTag(NbtTagType? type = null, bool readName = true)
        {
            type ??= ReadTagType();

            return type switch
            {
                NbtTagType.End => NbtEnd.FromReader(),
                NbtTagType.Byte => NbtByte.FromReader(this, readName),
                NbtTagType.Short => NbtShort.FromReader(this, readName),
                NbtTagType.Int => NbtInt.FromReader(this, readName),
                NbtTagType.Long => NbtLong.FromReader(this, readName),
                NbtTagType.Float => NbtFloat.FromReader(this, readName),
                NbtTagType.Double => NbtDouble.FromReader(this, readName),
                NbtTagType.ByteArray => NbtByteArray.FromReader(this, readName),
                NbtTagType.String => NbtString.FromReader(this, readName),
                NbtTagType.List => NbtList.FromReader(this, readName),
                NbtTagType.Compound => NbtCompound.FromReader(this, readName),
                NbtTagType.IntArray => NbtIntArray.FromReader(this, readName),
                NbtTagType.LongArray => NbtLongArray.FromReader(this, readName),
                null => NbtEnd.FromReader(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public byte ReadByte()
        {
            var data = _data[Position];
            Position++;

            return data;
        }

        public byte[] ReadArray(int length)
        {
            var data = _data[Position..(Position + length)];
            Position += length;

            return data;
        }

        public short ReadShort()
        {
            var data = ReadArray(2);
            return BinaryPrimitives.ReadInt16BigEndian(data);
        }

        public int ReadInt()
        {
            var data = ReadArray(4);
            return BinaryPrimitives.ReadInt32BigEndian(data);
        }

        public long ReadLong()
        {
            var data = ReadArray(8);
            return BinaryPrimitives.ReadInt64BigEndian(data);
        }

        public float ReadFloat()
        {
            var data = ReadArray(4);

            Array.Reverse(data);
            return BitConverter.ToSingle(data);
        }

        public double ReadDouble()
        {
            var data = ReadArray(8);

            Array.Reverse(data);
            return BitConverter.ToDouble(data);
        }

        public string ReadString()
        {
            var length = ReadShort();
            var data = ReadArray(length);

            return StringEncoder.GetString(data);
        }
    }
}