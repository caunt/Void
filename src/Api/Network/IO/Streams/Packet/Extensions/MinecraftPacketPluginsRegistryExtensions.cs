using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Extensions;

public static class MinecraftPacketPluginsRegistryExtensions
{
    public static bool TryGetTransformations(this IMinecraftPacketPluginsRegistry registriesHolder, IMinecraftPacketPluginsTransformations transformationsHolder, IMinecraftPacket packet, TransformationType transformationType, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
    {
        return TryGetTransformations(registriesHolder, transformationsHolder, packet.GetType(), transformationType, out transformations);
    }

    public static bool TryGetTransformations(this IMinecraftPacketPluginsRegistry registriesHolder, IMinecraftPacketPluginsTransformations transformationsHolder, Type packetType, TransformationType transformationType, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
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
