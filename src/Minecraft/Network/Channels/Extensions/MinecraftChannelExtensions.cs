using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries;
using Void.Minecraft.Network.Streams.Packet;

namespace Void.Minecraft.Network.Channels.Extensions;

public static class MinecraftChannelExtensions
{
    public static async ValueTask SendPacketAsync<T>(this INetworkChannel channel, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        await channel.WriteMessageAsync(packet, Side.Proxy, cancellationToken);
    }

    public static IRegistryHolder GetRegistries(this INetworkChannel channel)
    {
        if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream))
            return stream.Registries;

        throw new InvalidOperationException($"{nameof(IMinecraftPacketMessageStream)} is not found on this channel");
    }
}
