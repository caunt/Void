using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations;

public abstract record BaseTransformations(ProtocolVersion OlderVersion, ProtocolVersion NewerVersion)
{
    public abstract MinecraftPacketTransformationMapping[] Mappings { get; }
}
