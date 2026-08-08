using System;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Minecraft.Network.Registries.PacketId.Extensions;

/// <summary>
/// Connects plugin packet-ID ownership with plugin transformation lookup.
/// </summary>
public static class MinecraftPacketIdPluginsRegistryExtensions
{
    /// <summary>Attempts to find transformations contributed by the plugin that owns a packet's runtime type mapping.</summary>
    /// <param name="registriesHolder">The plugin packet-ID registries used to identify ownership.</param>
    /// <param name="transformationsHolder">The plugin transformation registries to query.</param>
    /// <param name="packet">The packet whose runtime type is queried.</param>
    /// <param name="transformationType">The upgrade or downgrade pipeline to retrieve.</param>
    /// <param name="transformations">When successful, the ordered transformation array; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when an owning plugin and exact transformation entry are found; otherwise, <see langword="false" />.</returns>
    public static bool TryGetTransformations(this IMinecraftPacketIdPluginsRegistry registriesHolder, IMinecraftPacketTransformationsPluginsRegistry transformationsHolder, IMinecraftPacket packet, TransformationType transformationType, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
    {
        return registriesHolder.TryGetTransformations(transformationsHolder, packet.GetType(), transformationType, out transformations);
    }

    /// <summary>Attempts to find transformations contributed by the plugin that owns a packet type mapping.</summary>
    /// <remarks>When packet ownership is found, the built-in implementation calls <see cref="IMinecraftPacketTransformationsPluginsRegistry.Get" />, which can create an empty per-plugin transformation registry as a lookup side effect.</remarks>
    /// <param name="registriesHolder">The plugin packet-ID registries used to identify ownership.</param>
    /// <param name="transformationsHolder">The plugin transformation registries to query.</param>
    /// <param name="packetType">The exact packet type whose transformation entry is requested.</param>
    /// <param name="transformationType">The upgrade or downgrade pipeline to retrieve.</param>
    /// <param name="transformations">When successful, the ordered transformation array; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when an owning plugin and exact transformation entry are found; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">Packet ownership is found but the transformation holder has no configured protocol version.</exception>
    public static bool TryGetTransformations(this IMinecraftPacketIdPluginsRegistry registriesHolder, IMinecraftPacketTransformationsPluginsRegistry transformationsHolder, Type packetType, TransformationType transformationType, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
    {
        transformations = null;

        if (!registriesHolder.TryGetPlugin(packetType, out var plugin))
            return false;

        if (!transformationsHolder.Get(plugin).TryGetFor(packetType, transformationType, out transformations))
            return false;

        return true;
    }
}
