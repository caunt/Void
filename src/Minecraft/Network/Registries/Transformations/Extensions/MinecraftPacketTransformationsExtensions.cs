using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Minecraft.Network.Registries.Transformations.Extensions;

/// <summary>
/// Extension methods for registering <see cref="MinecraftPacketTransformationMapping"/> sets with an <see cref="IMinecraftPacketTransformationsRegistry"/>.
/// </summary>
public static class MinecraftPacketTransformationsExtensions
{
    /// <summary>Adds one or more transformation mapping sequences for a packet type.</summary>
    /// <typeparam name="T">The packet type whose pipelines are registered.</typeparam>
    /// <param name="registry">The registry to update.</param>
    /// <param name="protocolVersion">The current protocol version used to filter mappings.</param>
    /// <param name="mappings">A sequence containing the transformation mappings for <typeparamref name="T" />.</param>
    /// <exception cref="ArgumentException"><typeparamref name="T" /> already has a registered transformation pipeline.</exception>
    public static void RegisterTransformations<T>(this IMinecraftPacketTransformationsRegistry registry, ProtocolVersion protocolVersion, params IEnumerable<MinecraftPacketTransformationMapping> mappings) where T : IMinecraftPacket
    {
        registry.Add(new Dictionary<IEnumerable<MinecraftPacketTransformationMapping>, Type>()
        {
            { mappings, typeof(T) }
        }, protocolVersion);
    }
}
