using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_12_1_to_v1_12_2;

public record KeepAliveTransformation1_12_2() : PacketTransformation(ProtocolVersion.MINECRAFT_1_12_1, ProtocolVersion.MINECRAFT_1_12_2)
{
    public override void Upgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        // VarInt => Long
        var id = wrapper.Read<VarIntProperty>();
        wrapper.Write(LongProperty.FromPrimitive(id.AsPrimitive));
    }

    public override void Downgrade(IMinecraftBinaryPacketWrapper wrapper)
    {
        // Long => VarInt
        var id = wrapper.Read<LongProperty>();
        wrapper.Write(VarIntProperty.FromPrimitive((int)id.AsPrimitive));
    }
}
