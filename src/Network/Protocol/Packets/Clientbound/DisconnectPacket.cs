using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Custom;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct DisconnectPacket : IMinecraftPacket<IPlayableState>
{
    public string Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteString(Reason);
    }

    public async Task<bool> HandleAsync(IPlayableState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
        Reason = buffer.ReadString(262144);
    }
}