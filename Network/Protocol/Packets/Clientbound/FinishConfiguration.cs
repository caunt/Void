using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public class FinishConfiguration : IMinecraftPacket<ConfigurationState>
{
    public void Encode(MinecraftBuffer buffer)
    {
    }

    public async Task<bool> HandleAsync(ConfigurationState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
    }
}