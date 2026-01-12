using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_20_2_to_v1_20_3;

public record PlayDisconnectTransformation1_20_3 : BaseTransformation1_20_3
{
    public override void Upgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        var jsonStringProperty = wrapper.Read<StringProperty>();
        var componentNbt = NbtJsonSerializer.Deserialize(jsonStringProperty.AsJsonNode);

        wrapper.Write(NbtProperty.FromNbtTag(componentNbt));
    }

    public override void Downgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        var namedNbtProperty = wrapper.Read<NbtProperty>();
        var componentJson = NbtJsonSerializer.Serialize(namedNbtProperty.AsNbtTag);

        wrapper.Write(StringProperty.FromJsonNode(componentJson));
    }
}
