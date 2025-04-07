using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Minecraft.Network.Streams.Packet;

namespace Void.Minecraft.Network.Channels.Extensions;

public static class MinecraftChannelExtensions
{
    public static async ValueTask SendPacketAsync<T>(this INetworkChannel channel, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        await channel.WriteMessageAsync(packet, Side.Proxy, cancellationToken);
    }

    public static IMinecraftPacketIdSystemRegistry GetPacketSystemRegistryHolder(this INetworkChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.SystemRegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketIdSystemRegistry)} is not set yet on this channel");
    }

    public static IMinecraftPacketIdPluginsRegistry GetPacketPluginsRegistryHolder(this INetworkChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.PluginsRegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketIdPluginsRegistry)} is not set yet on this channel");
    }

    public static IMinecraftPacketPluginsTransformations GetPacketTransformationsHolder(this INetworkChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.TransformationsHolder is { } transformations)
            return transformations;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketIdPluginsRegistry)} is not set yet on this channel");
    }
}
