using Void.Proxy.API.Players;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Extensions;

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

        if (stream.RegistryHolder is not { } registry)
            return false;

        return registry.Contains<SignedChatCommandPacket>() || registry.Contains<KeyedChatCommandPacket>() || registry.Contains<ChatMessagePacket>();
    }
}