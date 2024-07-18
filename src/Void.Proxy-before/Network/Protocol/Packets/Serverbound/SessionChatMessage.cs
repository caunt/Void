using System.Text;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.Packets.Shared;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Serverbound;

public struct SessionChatMessage : IMinecraftPacket<PlayState>, IChatMessage
{
    public string Message { get; set; }
    public long Timestamp { get; set; }
    public long Salt { get; set; }
    public byte[]? Signature { get; set; }
    public SessionLastSeenMessages SessionLastSeenMessages { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Message);
        buffer.WriteLong(Timestamp);
        buffer.WriteLong(Salt);
        buffer.WriteBoolean(Signature != null);

        if (Signature != null)
            buffer.Write(Signature);

        SessionLastSeenMessages.Encode(ref buffer);
    }

    public async Task<bool> HandleAsync(PlayState state)
    {
        return await state.HandleAsync(this);
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Message = buffer.ReadString(256);
        Timestamp = buffer.ReadLong();
        Salt = buffer.ReadLong();

        if (buffer.ReadBoolean())
            Signature = buffer.Read(256)
                .ToArray();

        SessionLastSeenMessages = new SessionLastSeenMessages(ref buffer);
    }

    public int MaxSize()
    {
        return 0 + Encoding.UTF8.GetByteCount(Message) + 8 + 8 + (Signature == null ? 0 : Signature.Length) + SessionLastSeenMessages.Acknowledged.Count;
    }
}