using System.Collections;
using Void.Proxy.Network.IO;

namespace Void.Proxy.Network.Protocol.Packets.Serverbound;

public class SessionLastSeenMessages
{
    public static int DivFloor(int dividend, int divisor) => dividend >= 0 ? dividend / divisor : dividend / divisor - (dividend % divisor == 0 ? 0 : 1);
    public static readonly int DIV_FLOOR = -DivFloor(-20, 8);

    public int Offset { get; set; }
    public BitArray Acknowledged { get; set; }
    public bool IsEmpty => !Acknowledged.HasAnySet();

    public SessionLastSeenMessages()
    {
        Offset = 0;
        Acknowledged = new(Array.Empty<byte>());
    }

    public SessionLastSeenMessages(ref MinecraftBuffer buffer)
    {
        Offset = buffer.ReadVarInt();

        var bytes = buffer.Read(DIV_FLOOR);
        Acknowledged = new(bytes.ToArray());
    }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(Offset);

        byte[] acknowledged = new byte[DIV_FLOOR];
        Acknowledged.CopyTo(acknowledged, 0);

        buffer.Write(acknowledged);
    }
}