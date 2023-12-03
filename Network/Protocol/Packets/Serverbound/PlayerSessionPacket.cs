using MinecraftProxy.Models;
using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public class PlayerSessionPacket : IMinecraftPacket<PlayState>
{
    public Guid SessionId { get; set; }
    public IdentifiedKey IdentifiedKey { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteGuid(SessionId);
        buffer.WriteIdentifiedKey(IdentifiedKey);
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        SessionId = buffer.ReadGuid();
        IdentifiedKey = buffer.ReadIdentifiedKey();
    }
}

