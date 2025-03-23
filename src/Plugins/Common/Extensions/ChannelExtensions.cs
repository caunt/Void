using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Extensions;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Plugins;

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

    public static void SetReadingPacketsMappings(this IMinecraftChannel channel, IPlugin plugin, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        var registry = channel.GetSystemPacketRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.ReplacePackets(Operation.Read, mappings);
    }

    public static void SetWritingPacketsMappings(this IMinecraftChannel channel, IPlugin plugin, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings)
    {
        var registry = channel.GetSystemPacketRegistryHolder();

        if (registry.ManagedBy != plugin)
            return;

        registry.ReplacePackets(Operation.Write, mappings);
    }

    public static void ClearPacketsMappings(this IMinecraftChannel channel, IPlugin plugin)
    {
        var systemRegistry = channel.GetSystemPacketRegistryHolder();
        var pluginsRegistry = channel.GetPluginsPacketRegistryHolder();

        if (systemRegistry.ManagedBy == plugin)
            systemRegistry.Reset();

        if (pluginsRegistry.ManagedBy == plugin)
            pluginsRegistry.Reset();
    }
}