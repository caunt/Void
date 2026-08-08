using System;
using System.Collections.Generic;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;

namespace Void.Minecraft.Network.Registries.PacketId.Extensions;

/// <summary>
/// Provides concise registration of one packet type and its identifier mappings.
/// </summary>
public static class MinecraftPacketIdRegistryExtensions
{
    /// <summary>Adds identifier mappings for a packet type at a protocol version.</summary>
    /// <typeparam name="T">The packet type to register.</typeparam>
    /// <param name="registry">The registry to update.</param>
    /// <param name="protocolVersion">The protocol version used to select an identifier.</param>
    /// <param name="mappings">The ordered identifier mappings for <typeparamref name="T" />.</param>
    /// <exception cref="ArgumentException">The selected mapping interval is invalid or conflicts with an existing identifier or type.</exception>
    public static void RegisterPacket<T>(this IMinecraftPacketIdRegistry registry, ProtocolVersion protocolVersion, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
    {
        registry.AddPackets(new Dictionary<MinecraftPacketIdMapping[], Type>()
        {
            { mappings, typeof(T) }
        }, protocolVersion);
    }
}
