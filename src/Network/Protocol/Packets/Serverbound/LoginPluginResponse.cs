using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct LoginPluginResponse : IMinecraftPacket<LoginState>
{
    public int MessageId { get; set; }
    public bool Successful { get; set; }
    public byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBoolean(Successful);
        buffer.Write(Data);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
        MessageId = buffer.ReadVarInt();
        Successful = buffer.ReadBoolean();
        Data = buffer.Read((int)(buffer.Length - buffer.Position)).ToArray();
    }
}