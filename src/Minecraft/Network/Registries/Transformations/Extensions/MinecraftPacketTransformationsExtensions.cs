using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Minecraft.Network.Registries.Transformations.Extensions;

public static class MinecraftPacketTransformationsExtensions
{
    public static void RegisterTransformations<T>(this IMinecraftPacketTransformationsRegistry registry, ProtocolVersion protocolVersion, params MinecraftPacketTransformationMapping[] mappings) where T : IMinecraftPacket
    {
        registry.AddTransformations(new Dictionary<MinecraftPacketTransformationMapping[], Type>()
        {
            { mappings, typeof(T) }
        }, protocolVersion);
    }
}
