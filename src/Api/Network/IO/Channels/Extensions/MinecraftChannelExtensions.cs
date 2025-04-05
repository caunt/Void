using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Api.Network.IO.Channels.Extensions;

public static class MinecraftChannelExtensions
{
    public static async ValueTask SendPacketAsync<T>(this IMinecraftChannel channel, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        await channel.WriteMessageAsync(packet, cancellationToken);
    }

    public static IMinecraftPacketSystemRegistry GetPacketSystemRegistryHolder(this IMinecraftChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.SystemRegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketSystemRegistry)} is not set yet on this channel");
    }

    public static IMinecraftPacketPluginsRegistry GetPacketPluginsRegistryHolder(this IMinecraftChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.PluginsRegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketPluginsRegistry)} is not set yet on this channel");
    }

    public static IMinecraftPacketPluginsTransformations GetPacketTransformationsHolder(this IMinecraftChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.TransformationsHolder is { } transformations)
            return transformations;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketPluginsRegistry)} is not set yet on this channel");
    }
}
