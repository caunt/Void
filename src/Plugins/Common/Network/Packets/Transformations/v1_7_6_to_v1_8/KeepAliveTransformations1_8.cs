using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_7_6_to_v1_8;

public record KeepAliveTransformations1_8 : BaseTransformations1_8
{
    public override void Upgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        // Int => VarInt
        var id = wrapper.Read<IntProperty>();
        wrapper.Write(VarIntProperty.FromPrimitive(id.AsPrimitive));
    }

    public override void Downgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        // VarInt => Int
        var id = wrapper.Read<VarIntProperty>();
        wrapper.Write(IntProperty.FromPrimitive(id.AsPrimitive));
    }
}
