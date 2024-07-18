using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.States.Common;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public struct HandshakePacket : IMinecraftPacket<HandshakeState>
{
    public int ProtocolVersion { get; set; }
    public string ServerAddress { get; set; }
    public ushort ServerPort { get; set; }
    public int NextState { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(ProtocolVersion);
        buffer.WriteString(ServerAddress);
        buffer.WriteUnsignedShort(ServerPort);
        buffer.WriteVarInt(NextState);
    }

    public async Task<bool> HandleAsync(HandshakeState state)
    {
        return await state.HandleAsync(this);
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        // var forgeMarker = ForgeMarker.Longest?.Value.Length + 1 ?? 0;

        ProtocolVersion = buffer.ReadVarInt();
        ServerAddress = buffer.ReadString(255 /* + forgeMarker*/);
        ServerPort = buffer.ReadUnsignedShort();
        NextState = buffer.ReadVarInt();
    }
}