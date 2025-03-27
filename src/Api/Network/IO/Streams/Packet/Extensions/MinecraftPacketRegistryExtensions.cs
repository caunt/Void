using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Extensions;

public static class MinecraftPacketRegistryExtensions
{
    public static void RegisterPacket<T>(this IMinecraftPacketRegistry registry, ProtocolVersion protocolVersion, params MinecraftPacketMapping[] mappings) where T : IMinecraftPacket
    {
        registry.AddPackets(new Dictionary<MinecraftPacketMapping[], Type>()
        {
            { mappings, typeof(T) }
        }, protocolVersion);
    }
}
