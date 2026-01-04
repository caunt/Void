using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_20_2_to_v1_20_3;

public record SystemChatMessageTransformations1_20_3() : BaseTransformations(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.MINECRAFT_1_20_3)
{
    public override MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, wrapper =>
        {
            var jsonStringProperty = wrapper.Read<StringProperty>();
            var componentNbt = ComponentNbtTransformers.Upgrade_v1_20_2_to_v1_20_3(NbtJsonSerializer.Deserialize(jsonStringProperty.AsJsonNode));

            wrapper.Write(NbtProperty.FromNbtTag(componentNbt));
            wrapper.Passthrough<BoolProperty>();
        }),
        new(NewerVersion, OlderVersion, wrapper =>
        {
            var namedNbtProperty = wrapper.Read<NbtProperty>();
            var componentNbt = ComponentNbtTransformers.Downgrade_v1_20_3_to_v1_20_2(namedNbtProperty.AsNbtTag);

            wrapper.Write(StringProperty.FromJsonNode(NbtJsonSerializer.Serialize(componentNbt)));
            wrapper.Passthrough<BoolProperty>();
        })
    ];
}
