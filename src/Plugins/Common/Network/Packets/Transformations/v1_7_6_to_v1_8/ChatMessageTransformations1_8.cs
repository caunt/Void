using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_7_6_to_v1_8;

public record ChatMessageTransformations1_8 : BaseTransformations1_8
{
    public override MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, wrapper =>
        {
            wrapper.Passthrough<StringProperty>();
            wrapper.Write(ByteProperty.FromPrimitive(1));
        }),
        new(NewerVersion, OlderVersion, wrapper =>
        {
            wrapper.Passthrough<StringProperty>();
            _ = wrapper.Read<ByteProperty>();
        })
    ];
}
