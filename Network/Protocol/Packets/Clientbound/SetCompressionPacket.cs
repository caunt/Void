using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct SetCompressionPacket : IMinecraftPacket<LoginState>
{
    public int Threshold { get; set; }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(Threshold);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
        Threshold = buffer.ReadVarInt();
    }
}