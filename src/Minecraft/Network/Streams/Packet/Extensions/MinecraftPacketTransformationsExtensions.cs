using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet.Transformations;

namespace Void.Minecraft.Network.Streams.Packet.Extensions;

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
