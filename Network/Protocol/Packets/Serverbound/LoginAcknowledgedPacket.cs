using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public struct LoginAcknowledgedPacket : IMinecraftPacket<LoginState>
{
    public void Encode(ref MinecraftBuffer buffer)
    {
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
    }
}

