using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_21_4_to_v1_21_5;

public record SystemChatMessageTransformations1_21_5() : BaseTransformations(ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_5)
{
    public override MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, wrapper =>
        {
            var property = wrapper.Read<NbtProperty>();
            var tag = ComponentNbtTransformers.Upgrade_v1_21_4_to_v1_21_5(property.AsNbtTag);

            wrapper.Write(NbtProperty.FromNbtTag(tag));
            wrapper.Passthrough<BoolProperty>();
        }),
        new(NewerVersion, OlderVersion, wrapper =>
        {
            var property = wrapper.Read<NbtProperty>();
            var tag = ComponentNbtTransformers.Downgrade_v1_21_5_to_v1_21_4(property.AsNbtTag);

            wrapper.Write(NbtProperty.FromNbtTag(tag));
            wrapper.Passthrough<BoolProperty>();
        })
    ];

    public override void Register(IPlayer player)
    {
        player.RegisterSystemTransformations<SystemChatMessagePacket>(Mappings);
    }
}
