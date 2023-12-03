using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public class LoginAcknowledgedPacket : IMinecraftPacket<LoginState>
{
    public void Encode(MinecraftBuffer buffer)
    {
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
    }
}

