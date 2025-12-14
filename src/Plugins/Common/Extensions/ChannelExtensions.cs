using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Minecraft.Network.Streams.Packet;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class ChannelExtensions
{
    public static async ValueTask<T> ReceivePacketAsync<T>(this INetworkChannel channel, Side origin, CancellationToken cancellationToken) where T : IMinecraftMessage
    {
        // Just for safety, ensure we have an IMinecraftPacket implementation in the channel registry
        if (!typeof(T).IsInterface)
        {
            var stream = channel.Get<IMinecraftPacketMessageStream>();

            if (!stream.Registries.PacketIdSystem.Contains<T>())
                throw new InvalidOperationException($"{nameof(stream.Registries.PacketIdSystem)} registry does not have {typeof(T)} packet");
        }

        var message = await channel.ReadMessageAsync(origin, cancellationToken);

        if (message is not T packet)
            throw new InvalidOperationException($"Received {message} packet is not {typeof(T)} packet");

        return packet;
    }

    public static void ReplaceSystemPackets(this INetworkChannel channel, Operation operation, IPlugin managedBy, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
    {
        channel.GetSystemRegistry(managedBy).ReplacePackets(operation, mappings);
    }

    public static void DisposeRegistries(this INetworkChannel channel, IPlugin managedBy)
    {
        var registries = channel.GetMinecraftRegistries();
        registries.DisposeBy(managedBy);
    }

    private static IMinecraftPacketIdSystemRegistry GetSystemRegistry(this INetworkChannel channel, IPlugin managedBy)
    {
        var registry = channel.GetMinecraftRegistries().PacketIdSystem;

        if (registry.ManagedBy != managedBy)
            throw new InvalidOperationException($"Registry is managed by {registry.ManagedBy}, not {managedBy.Name}");

        return registry;
    }
}
