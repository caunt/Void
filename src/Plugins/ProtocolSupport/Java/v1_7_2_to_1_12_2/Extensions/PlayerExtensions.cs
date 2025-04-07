using Void.Minecraft.Network.Streams.Packet;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask<bool> IsAuthenticatedAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        return await player.IsPlayingAsync(cancellationToken);
    }

    public static async ValueTask<bool> IsPlayingAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channel = await player.GetChannelAsync(cancellationToken);

        if (!channel.TryGet<IMinecraftPacketMessageStream>(out var stream))
            return false;

        if (stream.SystemRegistryHolder is not { } registry)
            return false;

        return registry.Contains<ChatMessagePacket>();
    }

    public static async ValueTask<bool> IsLoggingInAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channel = await player.GetChannelAsync(cancellationToken);

        if (!channel.TryGet<IMinecraftPacketMessageStream>(out var stream))
            return false;

        if (stream.SystemRegistryHolder is not { } registry)
            return false;

        // if registry contains one of Login state packet
        return registry.Contains<LoginDisconnectPacket>();
    }
}
