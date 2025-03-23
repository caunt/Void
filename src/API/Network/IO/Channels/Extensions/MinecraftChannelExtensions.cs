using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.IO.Streams.Packet.Registries;

namespace Void.Proxy.API.Network.IO.Channels.Extensions;

public static class MinecraftChannelExtensions
{
    public static async ValueTask SendPacketAsync<T>(this IMinecraftChannel channel, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        await channel.WriteMessageAsync(packet, cancellationToken);
    }

    public static IMinecraftPacketRegistrySystem GetSystemPacketRegistryHolder(this IMinecraftChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.SystemRegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketRegistrySystem)} is not set yet on this channel");
    }

    public static IMinecraftPacketRegistryPlugins GetPluginsPacketRegistryHolder(this IMinecraftChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.PluginsRegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketRegistryPlugins)} is not set yet on this channel");
    }
}
