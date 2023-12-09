using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.Packets.Shared;
using MinecraftProxy.Network.Protocol.States.Common;
using System.Collections;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public struct SessionChatMessage : IMinecraftPacket<PlayState>, IChatMessage
{
    public string Message { get; set; }
    public long Timestamp { get; set; }
    public long Salt { get; set; }
    public byte[]? Signature { get; set; }
    public LastSeenMessages LastSeenMessages { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Message);
        buffer.WriteLong(Timestamp);
        buffer.WriteLong(Salt);
        buffer.WriteBoolean(Signature != null);

        if (Signature != null)
            buffer.Write(Signature);

        LastSeenMessages.Encode(ref buffer);
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Message = buffer.ReadString(256);
        Timestamp = buffer.ReadLong();
        Salt = buffer.ReadLong();

        if (buffer.ReadBoolean())
            Signature = buffer.Read(256).ToArray();

        LastSeenMessages = new LastSeenMessages(ref buffer);
    }
}

public class LastSeenMessages
{
    public static int DivFloor(int dividend, int divisor) => dividend >= 0 ? dividend / divisor : dividend / divisor - (dividend % divisor == 0 ? 0 : 1);
    public static readonly int DIV_FLOOR = -DivFloor(-20, 8);

    public int Offset { get; set; }
    public BitArray Acknowledged { get; set; }
    public bool IsEmpty => !Acknowledged.HasAnySet();

    public LastSeenMessages()
    {
        Offset = 0;
        Acknowledged = new(Array.Empty<byte>());
    }

    public LastSeenMessages(ref MinecraftBuffer buffer)
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