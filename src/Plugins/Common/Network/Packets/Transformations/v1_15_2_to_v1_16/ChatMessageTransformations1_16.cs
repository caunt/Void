using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_15_2_to_v1_16;

public record ChatMessageTransformations1_16() : BaseTransformations(ProtocolVersion.MINECRAFT_1_15_2, ProtocolVersion.MINECRAFT_1_16)
{
    public override void Upgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        ComponentJsonTransformers.Passthrough_v1_15_2_to_v1_16(wrapper);
        wrapper.Passthrough<ByteProperty>();
        wrapper.Write(UuidProperty.Empty);
    }

    public override void Downgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        ComponentJsonTransformers.Passthrough_v1_16_to_v1_15_2(wrapper);
        wrapper.Passthrough<ByteProperty>();
        _ = wrapper.Read<UuidProperty>();
    }
}
