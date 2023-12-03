namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public class DisconnectPacket : IMinecraftPacket<IPlayableState>
{
    public string Reason { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteString(Reason);
    }

    public async Task<bool> HandleAsync(IPlayableState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        Reason = buffer.ReadString(262144);
    }
}