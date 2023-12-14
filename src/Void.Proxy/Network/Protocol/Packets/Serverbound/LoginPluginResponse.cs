using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

public struct LoginPluginResponse : IMinecraftPacket<LoginState>
{
    public int MessageId { get; set; }
    public bool Successful { get; set; }
    public byte[] Data { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBoolean(Successful);
        buffer.Write(Data);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        MessageId = buffer.ReadVarInt();
        Successful = buffer.ReadBoolean();
        Data = buffer.Read((int)(buffer.Length - buffer.Position)).ToArray();
    }

    public int MaxSize() => 0
        + 5
        + 1
        + Data.Length + 5;
}