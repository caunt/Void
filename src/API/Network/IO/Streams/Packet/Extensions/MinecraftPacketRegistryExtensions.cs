using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.API.Network.IO.Streams.Packet.Extensions;

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
