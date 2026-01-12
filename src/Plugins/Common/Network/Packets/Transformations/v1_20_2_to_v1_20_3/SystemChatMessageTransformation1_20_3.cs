using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_20_2_to_v1_20_3;

public record SystemChatMessageTransformation1_20_3 : BaseTransformation1_20_3
{
    public override void Upgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        var jsonStringProperty = wrapper.Read<StringProperty>();
        var componentNbt = ComponentNbtTransformers.Upgrade_v1_20_2_to_v1_20_3(NbtJsonSerializer.Deserialize(jsonStringProperty.AsJsonNode));

        wrapper.Write(NbtProperty.FromNbtTag(componentNbt));
        wrapper.Passthrough<BoolProperty>();
    }

    public override void Downgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        var namedNbtProperty = wrapper.Read<NbtProperty>();
        var componentNbt = ComponentNbtTransformers.Downgrade_v1_20_3_to_v1_20_2(namedNbtProperty.AsNbtTag);

        wrapper.Write(StringProperty.FromJsonNode(NbtJsonSerializer.Serialize(componentNbt)));
        wrapper.Passthrough<BoolProperty>();
    }
}
