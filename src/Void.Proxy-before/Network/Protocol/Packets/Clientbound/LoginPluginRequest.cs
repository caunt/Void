using System.Text;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

public struct LoginPluginRequest : IMinecraftPacket<LoginState>
{
    public int MessageId { get; set; }
    public string Identifier { get; set; }
    public byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteString(Identifier);
        buffer.Write(Data);
    }

    public async Task<bool> HandleAsync(LoginState state)
    {
        return await state.HandleAsync(this);
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        MessageId = buffer.ReadVarInt();
        Identifier = buffer.ReadString();
        Data = buffer.Read((int)(buffer.Length - buffer.Position))
            .ToArray();
    }

    public int MaxSize()
    {
        return 0 + 5 + Encoding.UTF8.GetByteCount(Identifier) + 5 + Data.Length + 5;
    }
}