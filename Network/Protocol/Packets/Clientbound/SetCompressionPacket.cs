using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public class SetCompressionPacket : IMinecraftPacket<LoginState>
{
    public int Threshold { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(Threshold);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        Threshold = buffer.ReadVarInt();
    }
}