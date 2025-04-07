using Void.Common.Network;
using Void.Common.Plugins;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Network.IO.Channels.Extensions;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class ChannelExtensions
{
    public static async ValueTask<T> ReceivePacketAsync<T>(this IMinecraftChannel channel, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        // just for safety, ensure we do have such IMinecraftPacket implementation in channel registry
        if (!typeof(T).IsInterface)
        {
            var stream = channel.Get<IMinecraftPacketMessageStream>();

            if (stream.SystemRegistryHolder is null)
                throw new InvalidOperationException($"{nameof(IMinecraftChannel)}.{nameof(IMinecraftPacketMessageStream)} does not have packet registry");

            if (!stream.SystemRegistryHolder.Contains<T>())
                throw new InvalidOperationException($"{nameof(IMinecraftChannel)}.{nameof(IMinecraftPacketMessageStream)} registry does not have {typeof(T)} packet");
        }

        var message = await channel.ReadMessageAsync(cancellationToken);

        if (message is not T packet)
            throw new InvalidOperationException($"Received {message} packet is not {typeof(T)} packet");

        return packet;
    }

    public static void SetReadingPacketsMappings(this IMinecraftChannel channel, IPlugin plugin, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
    {
        var registry = channel.GetPacketSystemRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.ReplacePackets(Operation.Read, mappings);
    }

    public static void SetWritingPacketsMappings(this IMinecraftChannel channel, IPlugin plugin, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
    {
        var registry = channel.GetPacketSystemRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.ReplacePackets(Operation.Write, mappings);
    }

    public static void ClearPluginsHolders(this IMinecraftChannel channel, IPlugin managedBy)
    {
        var systemRegistry = channel.GetPacketSystemRegistryHolder();
        var pluginsRegistry = channel.GetPacketPluginsRegistryHolder();
        var transformations = channel.GetPacketTransformationsHolder();

        if (systemRegistry.ManagedBy == managedBy)
            systemRegistry.Reset();

        if (pluginsRegistry.ManagedBy == managedBy)
            pluginsRegistry.Reset();

        if (transformations.ManagedBy == managedBy)
            transformations.Reset();
    }
}
