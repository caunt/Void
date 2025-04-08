using System;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Minecraft.Network.Registries.PacketId.Extensions;

public static class MinecraftPacketIdPluginsRegistryExtensions
{
    public static bool TryGetTransformations(this IMinecraftPacketIdPluginsRegistry registriesHolder, IMinecraftPacketTransformationsPluginsRegistry transformationsHolder, IMinecraftPacket packet, TransformationType transformationType, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
    {
        return registriesHolder.TryGetTransformations(transformationsHolder, packet.GetType(), transformationType, out transformations);
    }

    public static bool TryGetTransformations(this IMinecraftPacketIdPluginsRegistry registriesHolder, IMinecraftPacketTransformationsPluginsRegistry transformationsHolder, Type packetType, TransformationType transformationType, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
    {
        transformations = null;

        if (!registriesHolder.TryGetPlugin(packetType, out var plugin))
            return false;

        if (!transformationsHolder.Get(plugin).TryGetTransformations(packetType, transformationType, out transformations))
            return false;

        return true;
    }
}
