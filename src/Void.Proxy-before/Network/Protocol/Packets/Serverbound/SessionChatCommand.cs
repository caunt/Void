using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.Packets.Shared;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Serverbound;

public struct SessionChatCommand : IMinecraftPacket<PlayState>, IChatCommand
{
    public string Command { get; set; }
    public long Timestamp { get; set; }
    public long Salt { get; set; }
    public Dictionary<string, byte[]> Arguments { get; set; }
    public SessionLastSeenMessages SessionLastSeenMessages { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Command);
        buffer.WriteLong(Timestamp);
        buffer.WriteLong(Salt);

        buffer.WriteVarInt(Arguments.Count);
        foreach (var (argument, signature) in Arguments)
        {
            buffer.WriteString(argument);
            buffer.Write(signature);
        }

        SessionLastSeenMessages.Encode(ref buffer);
    }

    public async Task<bool> HandleAsync(PlayState state)
    {
        return await state.HandleAsync(this);
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Command = buffer.ReadString(256);
        Timestamp = buffer.ReadLong();
        Salt = buffer.ReadLong();

        var size = buffer.ReadVarInt();
        if (size > 8)
            throw new Exception($"Too many argument signatures, {size} is above limit 8");

        Arguments = new Dictionary<string, byte[]>(size);
        for (var i = 0; i < size; i++)
            Arguments.Add(buffer.ReadString(), buffer.Read(256).ToArray());

        SessionLastSeenMessages = new SessionLastSeenMessages(ref buffer);
    }
}