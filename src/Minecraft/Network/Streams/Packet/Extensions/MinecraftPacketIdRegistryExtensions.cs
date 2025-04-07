using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet.Registries;

namespace Void.Minecraft.Network.Streams.Packet.Extensions;

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
