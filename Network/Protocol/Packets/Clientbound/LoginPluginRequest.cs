using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct LoginPluginRequest : IMinecraftPacket<LoginState>
{
    public int MessageId { get; set; }
    public string Identifier { get; set; }
    public byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteString(Identifier);
        buffer.Write(Data);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
        MessageId = buffer.ReadVarInt();
        Identifier = buffer.ReadString();
        Data = buffer.Read((int)(buffer.Length - buffer.Position)).ToArray();
    }
}