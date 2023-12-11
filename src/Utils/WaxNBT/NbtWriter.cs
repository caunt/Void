using System.Buffers.Binary;
using System.Text;

namespace MinecraftProxy.Utils.WaxNBT;

public class NbtWriter
{
    public Encoding StringEncoder = Encoding.UTF8;

    private readonly MemoryStream _stream = new();

    public void Write(NbtTagType type) => Write((byte)type);

    public void Write(byte value) => _stream.WriteByte(value);

    public void Write(byte[] data) => _stream.Write(data);

    public void Write(short value)
    {
        byte[] data = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(data, value);

        _stream.Write(data, 0, 2);
    }

    public void Write(int value)
    {
        byte[] data = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(data, value);

        _stream.Write(data, 0, 4);
    }

    public void Write(long value)
    {
        byte[] data = new byte[8];
        BinaryPrimitives.WriteInt64BigEndian(data, value);

        _stream.Write(data, 0, 8);
    }

    public void Write(float value)
    {
        byte[] data = BitConverter.GetBytes(value);
        Array.Reverse(data);
        _stream.Write(data);
    }

    public void Write(double value)
    {
        byte[] data = BitConverter.GetBytes(value);
        Array.Reverse(data);
        _stream.Write(data);
    }

    public void Write(string value)
    {
        byte[] data = StringEncoder.GetBytes(value);

        Write((short)value.Length);
        _stream.Write(data);
    }

    public Stream GetStream() => _stream;
}