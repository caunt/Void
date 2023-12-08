using MinecraftProxy.Models;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public struct PlayerSessionPacket : IMinecraftPacket<PlayState>
{
    public Guid SessionId { get; set; }
    public IdentifiedKey IdentifiedKey { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteGuid(SessionId);
        buffer.WriteIdentifiedKey(IdentifiedKey);
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        SessionId = buffer.ReadGuid();
        IdentifiedKey = buffer.ReadIdentifiedKey(protocolVersion);
    }
}

