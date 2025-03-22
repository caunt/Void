using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;
using Void.Proxy.Plugins.Common.Registries.Packets;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class ChannelExtensions
{
    public static async ValueTask<T> ReceivePacketAsync<T>(this IMinecraftChannel channel, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        // just for safety, ensure we do have such IMinecraftPacket implementation in channel registry
        if (!typeof(T).IsInterface)
        {
            var stream = channel.Get<IMinecraftPacketMessageStream>();

            if (stream.RegistryHolder is null)
                throw new InvalidOperationException($"{nameof(IMinecraftChannel)}.{nameof(IMinecraftPacketMessageStream)} does not have packet registry");

            if (!stream.RegistryHolder.Contains<T>())
                throw new InvalidOperationException($"{nameof(IMinecraftChannel)}.{nameof(IMinecraftPacketMessageStream)} registry does not have {typeof(T)} packet");
        }

        var message = await channel.ReadMessageAsync(cancellationToken);

        if (message is not T packet)
            throw new InvalidOperationException($"Received {message} packet is not {typeof(T)} packet");

        return packet;
    }

    public static async ValueTask SendPacketAsync<T>(this IMinecraftChannel channel, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        await channel.WriteMessageAsync(packet, cancellationToken);
    }

    public static void SetReadingPacketsMappings(this IMinecraftChannel channel, IPlugin plugin, IReadOnlyDictionary<PacketMapping[], Type> mappings)
    {
        var registry = channel.GetPacketRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.ReplacePackets(Operation.Read, mappings);
    }

    public static void SetWritingPacketsMappings(this IMinecraftChannel channel, IPlugin plugin, IReadOnlyDictionary<PacketMapping[], Type> mappings)
    {
        var registry = channel.GetPacketRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.ReplacePackets(Operation.Write, mappings);
    }

    public static void ClearPacketsMappings(this IMinecraftChannel channel, IPlugin plugin)
    {
        var registry = channel.GetPacketRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.Reset();
    }

    public static IPacketRegistryHolder GetPacketRegistryHolder(this IMinecraftChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.RegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IPacketRegistryHolder)} is not set yet on this channel");
    }
}