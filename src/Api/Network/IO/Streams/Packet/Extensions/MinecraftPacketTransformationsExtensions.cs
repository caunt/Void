using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Extensions;

public static class MinecraftPacketTransformationsExtensions
{
    public static void RegisterTransformations<T>(this IMinecraftPacketTransformations registry, ProtocolVersion protocolVersion, params MinecraftPacketTransformationMapping[] mappings) where T : IMinecraftPacket
    {
        registry.AddTransformations(new Dictionary<MinecraftPacketTransformationMapping[], Type>()
        {
            { mappings, typeof(T) }
        }, protocolVersion);
    }
}
