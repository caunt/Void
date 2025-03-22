using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Void.NBT
{
    public class NbtWriter
    {
        private readonly MemoryStream _stream = new MemoryStream();
        public Encoding StringEncoder = Encoding.UTF8;

        public void Write(NbtTagType type)
        {
            Write((byte)type);
        }

        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public void Write(byte[] data)
        {
            _stream.Write(data);
        }

        public void Write(short value)
        {
            var data = new byte[2];
            BinaryPrimitives.WriteInt16BigEndian(data, value);

            _stream.Write(data, 0, 2);
        }

        public void Write(int value)
        {
            var data = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(data, value);

            _stream.Write(data, 0, 4);
        }

        public void Write(long value)
        {
            var data = new byte[8];
            BinaryPrimitives.WriteInt64BigEndian(data, value);

            _stream.Write(data, 0, 8);
        }

        public void Write(float value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            _stream.Write(data);
        }

        public void Write(double value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            _stream.Write(data);
        }

        public void Write(string value)
        {
            var data = StringEncoder.GetBytes(value);

            Write((short)value.Length);
            _stream.Write(data);
        }

        public Stream GetStream()
        {
            _stream.Position = 0;
            return _stream;
        }
    }
}