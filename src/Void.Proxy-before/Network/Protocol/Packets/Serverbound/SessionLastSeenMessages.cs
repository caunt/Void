using System.Collections;
using Void.Proxy.Network.IO;

namespace Void.Proxy.Network.Protocol.Packets.Serverbound;

public class SessionLastSeenMessages
{
    public static readonly int DIV_FLOOR = -DivFloor(-20, 8);

    public SessionLastSeenMessages()
    {
        Offset = 0;
        Acknowledged = new BitArray(Array.Empty<byte>());
    }

    public SessionLastSeenMessages(ref MinecraftBuffer buffer)
    {
        Offset = buffer.ReadVarInt();

        var bytes = buffer.Read(DIV_FLOOR);
        Acknowledged = new BitArray(bytes.ToArray());
    }

    public int Offset { get; set; }
    public BitArray Acknowledged { get; set; }
    public bool IsEmpty => !Acknowledged.HasAnySet();

    public static int DivFloor(int dividend, int divisor)
    {
        return dividend >= 0 ? dividend / divisor : dividend / divisor - (dividend % divisor == 0 ? 0 : 1);
    }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(Offset);

        var acknowledged = new byte[DIV_FLOOR];
        Acknowledged.CopyTo(acknowledged, 0);

        buffer.Write(acknowledged);
    }
}