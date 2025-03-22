using Void.Proxy.API.Players;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask<bool> IsAuthenticatedAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        return await player.IsPlayingAsync(cancellationToken) || await player.IsConfiguringAsync(cancellationToken);
    }

    public static async ValueTask<bool> IsPlayingAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channel = await player.GetChannelAsync(cancellationToken);

        if (!channel.TryGet<IMinecraftPacketMessageStream>(out var stream))
            return false;

        if (stream.RegistryHolder is not { } registry)
            return false;

        // if registry contains one of Play state packet
        return registry.Contains<StartConfigurationPacket>();
    }

    public static async ValueTask<bool> IsConfiguringAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channel = await player.GetChannelAsync(cancellationToken);

        if (!channel.TryGet<IMinecraftPacketMessageStream>(out var stream))
            return false;

        if (stream.RegistryHolder is not { } registry)
            return false;

        // if registry contains one of Configuration state packet
        return registry.Contains<AcknowledgeFinishConfigurationPacket>();
    }
}