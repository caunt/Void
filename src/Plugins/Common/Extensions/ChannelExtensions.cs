using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Common.Plugins;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Streams.Packet;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class ChannelExtensions
{
    public static async ValueTask<T> ReceivePacketAsync<T>(this INetworkChannel channel, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        // just for safety, ensure we do have such IMinecraftPacket implementation in channel registry
        if (!typeof(T).IsInterface)
        {
            var stream = channel.Get<IMinecraftPacketMessageStream>();

            if (!stream.Registries.SystemRegistryHolder.Contains<T>())
                throw new InvalidOperationException($"{nameof(stream.Registries.SystemRegistryHolder)} registry does not have {typeof(T)} packet");
        }

        var message = await channel.ReadMessageAsync(cancellationToken);

        if (message is not T packet)
            throw new InvalidOperationException($"Received {message} packet is not {typeof(T)} packet");

        return packet;
    }

    public static void SetReadingPacketsMappings(this INetworkChannel channel, IPlugin managedBy, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
    {
        channel.GetSystemRegistry(managedBy).ReplacePackets(Operation.Read, mappings);
    }

    public static void SetWritingPacketsMappings(this INetworkChannel channel, IPlugin managedBy, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
    {
        channel.GetSystemRegistry(managedBy).ReplacePackets(Operation.Write, mappings);
    }

    public static void DisposeRegistries(this INetworkChannel channel, IPlugin managedBy)
    {
        var registries = channel.GetRegistries();
        registries.DisposeBy(managedBy);
    }

    private static IMinecraftPacketIdSystemRegistry GetSystemRegistry(this INetworkChannel channel, IPlugin managedBy)
    {
        var registry = channel.GetRegistries().SystemRegistryHolder;

        if (registry.ManagedBy != managedBy)
            throw new InvalidOperationException($"Registry is managed by {registry.ManagedBy}, not {managedBy.Name}");

        return registry;
    }
}
