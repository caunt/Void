using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_7_6_to_v1_8;

public record KeepAliveTransformations1_8 : BaseTransformations1_8
{
    public override MinecraftPacketTransformationMapping[] Mappings => [
        new(OlderVersion, NewerVersion, wrapper =>
        {
            // Int => VarInt
            var id = wrapper.Read<IntProperty>();
            wrapper.Write(VarIntProperty.FromPrimitive(id.AsPrimitive));
        }),
        new(NewerVersion, OlderVersion, wrapper =>
        {
            // VarInt => Int
            var id = wrapper.Read<VarIntProperty>();
            wrapper.Write(IntProperty.FromPrimitive(id.AsPrimitive));
        })
    ];

    public override void Register(IPlayer player)
    {
        player.RegisterSystemTransformations<KeepAliveRequestPacket>(Mappings);
        player.RegisterSystemTransformations<KeepAliveResponsePacket>(Mappings);
    }
}
