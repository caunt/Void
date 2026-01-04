using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_11_1_to_v1_12;

public record ChatMessageTransformations1_12() : BaseTransformations(ProtocolVersion.MINECRAFT_1_11_1, ProtocolVersion.MINECRAFT_1_12)
{
    public override void Upgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        ComponentJsonTransformers.Passthrough_v1_11_1_to_v1_12(wrapper);
        wrapper.Passthrough<ByteProperty>();
    }

    public override void Downgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        ComponentJsonTransformers.Passthrough_v1_12_to_v1_11_1(wrapper);
        wrapper.Passthrough<ByteProperty>();
    }
}
