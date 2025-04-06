using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Extensions;

public static class MinecraftPacketIdRegistryExtensions
{
    public static void RegisterPacket<T>(this IMinecraftPacketIdRegistry registry, ProtocolVersion protocolVersion, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
    {
        registry.AddPackets(new Dictionary<MinecraftPacketIdMapping[], Type>()
        {
            { mappings, typeof(T) }
        }, protocolVersion);
    }
}
