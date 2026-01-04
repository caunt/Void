using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations;

public abstract record BaseTransformations(ProtocolVersion OlderVersion, ProtocolVersion NewerVersion)
{
    public MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, Upgrade),
        new(NewerVersion, OlderVersion, Downgrade)
    ];

    public abstract void Upgrade(IMinecraftBinaryPacketWrapper wrapper);
    public abstract void Downgrade(IMinecraftBinaryPacketWrapper wrapper);
}
