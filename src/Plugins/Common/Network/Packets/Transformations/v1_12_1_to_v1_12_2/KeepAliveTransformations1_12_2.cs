using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_12_1_to_v1_12_2;

public record KeepAliveTransformations1_12_2() : BaseTransformations(ProtocolVersion.MINECRAFT_1_12_1, ProtocolVersion.MINECRAFT_1_12_2)
{
    public override MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, wrapper =>
        {
            // VarInt => Long
            var id = wrapper.Read<VarIntProperty>();
            wrapper.Write(LongProperty.FromPrimitive(id.AsPrimitive));
        }),
        new(NewerVersion, OlderVersion, wrapper =>
        {
            // Long => VarInt
            var id = wrapper.Read<LongProperty>();
            wrapper.Write(VarIntProperty.FromPrimitive((int)id.AsPrimitive));
        })
    ];
}
