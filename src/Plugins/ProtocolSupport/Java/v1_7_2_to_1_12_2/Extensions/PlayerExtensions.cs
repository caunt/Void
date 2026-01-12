using Void.Minecraft.Network.Streams.Packet;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

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

        var registry = stream.Registries.PacketIdSystem;

        return registry.Contains<Packets.Serverbound.ChatMessagePacket>();
    }

    public static async ValueTask<bool> IsLoggingInAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channel = await player.GetChannelAsync(cancellationToken);

        if (!channel.TryGet<IMinecraftPacketMessageStream>(out var stream))
            return false;

        var registry = stream.Registries.PacketIdSystem;

        // if registry contains one of Login state packets
        return registry.Contains<LoginSuccessPacket>();
    }
}
