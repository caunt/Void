using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_21_4_to_v1_21_5;

public record SystemChatMessageTransformation1_21_5() : PacketTransformation(ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_5)
{
    public override void Upgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = ComponentNbtTransformers.Upgrade_v1_21_4_to_v1_21_5(property.AsNbtTag);

        wrapper.Write(NbtProperty.FromNbtTag(tag));
        wrapper.Passthrough<BoolProperty>();
    }

    public override void Downgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = ComponentNbtTransformers.Downgrade_v1_21_5_to_v1_21_4(property.AsNbtTag);

        wrapper.Write(NbtProperty.FromNbtTag(tag));
        wrapper.Passthrough<BoolProperty>();
    }
}
