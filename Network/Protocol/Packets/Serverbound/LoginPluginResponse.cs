using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public class LoginPluginResponse : IMinecraftPacket<LoginState>
{
    public int MessageId { get; set; }
    public bool Successful { get; set; }
    public byte[] Data { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBoolean(Successful);
        buffer.Write(Data);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        MessageId = buffer.ReadVarInt();
        Successful = buffer.ReadBoolean();
        Data = buffer.ReadUInt8Array((int)(buffer.Length - buffer.Position));
    }
}