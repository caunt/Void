using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;

namespace Void.Proxy.API.Network.IO.Channels.Extensions;

public static class MinecraftChannelExtensions
{
    public static async ValueTask SendPacketAsync<T>(this IMinecraftChannel channel, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        await channel.WriteMessageAsync(packet, cancellationToken);
    }

    // Obsolete
    // public static void RegisterPacket<T>(this IMinecraftChannel channel, params MinecraftPacketMapping[] mappings)
    // {
    //     var registry = channel.GetPacketRegistryHolder();
    //     registry.AddPackets(Operation.Any, new Dictionary<MinecraftPacketMapping[], Type>()
    //     {
    //         { mappings, typeof(T) }
    //     });
    // }

    public static IMinecraftPacketRegistryHolder GetPacketRegistryHolder(this IMinecraftChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream) && stream.RegistryHolder is { } registry)
            return registry;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketRegistryHolder)} is not set yet on this channel");
    }
}
