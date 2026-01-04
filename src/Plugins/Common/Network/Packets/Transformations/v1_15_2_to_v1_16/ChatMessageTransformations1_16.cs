using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_15_2_to_v1_16;

public record ChatMessageTransformations1_16() : BaseTransformations(ProtocolVersion.MINECRAFT_1_15_2, ProtocolVersion.MINECRAFT_1_16)
{
    public override MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_15_2_to_v1_16(wrapper);
            wrapper.Passthrough<ByteProperty>();
            wrapper.Write(UuidProperty.Empty);
        }),
        new(NewerVersion, OlderVersion, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_16_to_v1_15_2(wrapper);
            wrapper.Passthrough<ByteProperty>();
            _ = wrapper.Read<UuidProperty>();
        })
    ];

    public override void Register(IPlayer player)
    {
        player.RegisterSystemTransformations<ChatMessagePacket>(Mappings);
    }
}
