using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_8_to_v1_9;

public record ChatMessageTransformations1_9() : BaseTransformations(ProtocolVersion.MINECRAFT_1_8, ProtocolVersion.MINECRAFT_1_9)
{
    public override MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_8_to_v1_9(wrapper);
            wrapper.Passthrough<ByteProperty>();
        }),
        new(NewerVersion, OlderVersion, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_9_to_v1_8(wrapper);
            wrapper.Passthrough<ByteProperty>();
        })
    ];
}
