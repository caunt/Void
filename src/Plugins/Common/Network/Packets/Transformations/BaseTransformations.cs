using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations;

public abstract record BaseTransformations(ProtocolVersion OlderVersion, ProtocolVersion NewerVersion)
{
    public abstract MinecraftPacketTransformationMapping[] Mappings { get; }
    public abstract void Register(IPlayer player);
}
