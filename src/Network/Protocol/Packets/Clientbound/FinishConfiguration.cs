using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct FinishConfiguration : IMinecraftPacket<ConfigurationState>
{
    public void Encode(ref MinecraftBuffer buffer)
    {
    }

    public async Task<bool> HandleAsync(ConfigurationState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
    }
}