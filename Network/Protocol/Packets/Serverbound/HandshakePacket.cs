using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public class HandshakePacket : IMinecraftPacket<HandshakeState>
{
    public int ProtocolVersion { get; set; }
    public string ServerAddress { get; set; }
    public ushort ServerPort { get; set; }
    public int NextState { get; set; }

    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(ProtocolVersion);
        buffer.WriteString(ServerAddress);
        buffer.WriteUnsignedShort(ServerPort);
        buffer.WriteVarInt(NextState);
    }

    public async Task<bool> HandleAsync(HandshakeState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        ProtocolVersion = buffer.ReadVarInt();
        ServerAddress = buffer.ReadString();
        ServerPort = buffer.ReadUnsignedShort();
        NextState = buffer.ReadVarInt();
    }
}