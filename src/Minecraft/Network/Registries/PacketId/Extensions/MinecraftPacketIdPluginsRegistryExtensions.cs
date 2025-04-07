using System;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations;

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

        if (registriesHolder is null)
            return false;

        if (!registriesHolder.TryGetPlugin(packetType, out var plugin))
            return false;

        if (transformationsHolder is null)
            return false;

        if (!transformationsHolder.Get(plugin).TryGetTransformation(packetType, transformationType, out transformations))
            return false;

        return true;
    }
}
