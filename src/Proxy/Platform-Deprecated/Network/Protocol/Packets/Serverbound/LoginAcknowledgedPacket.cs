using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Serverbound;

public struct LoginAcknowledgedPacket : IMinecraftPacket<LoginState>
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
    }

    public async Task<bool> HandleAsync(LoginState state)
    {
        return await state.HandleAsync(this);
    }

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
    }
}