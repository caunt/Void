using Void.Minecraft.Network;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Extensions;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_11_1_to_v1_12;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_12_1_to_v1_12_2;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_15_2_to_v1_16;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_20_2_to_v1_20_3;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_21_4_to_v1_21_5;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_7_6_to_v1_8;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_8_to_v1_9;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations;

public static class NetworkTransformations
{
    public static PacketTransformation[] KeepAlive { get; } = [
        new KeepAliveTransformation1_8(),
        new KeepAliveTransformation1_12_2()
    ];

    public static PacketTransformation[] ChatMessage { get; } = [
        new ChatMessageTransformation1_8(),
        new ChatMessageTransformation1_9(),
        new ChatMessageTransformation1_12(),
        new ChatMessageTransformation1_16()
    ];

    public static PacketTransformation[] SystemChatMessage { get; } = [
        new SystemChatMessageTransformation1_20_3(),
        new SystemChatMessageTransformation1_21_5()
    ];

    public static PacketTransformation[] PlayDisconnect { get; } = [
        new PlayDisconnectTransformation1_20_3()
    ];

    public static void Register(ILink? link, IPlayer player, Phase phase)
    {
        // Not really possible, but just to be sure
        link ??= player.Link;

        ArgumentNullException.ThrowIfNull(link, nameof(link));

        switch (phase)
        {
            case Phase.Play:
                RegisterMappings<KeepAliveRequestPacket>(link, KeepAlive.SelectMany(keepAlive => keepAlive.Mappings));
                RegisterMappings<KeepAliveResponsePacket>(link, KeepAlive.SelectMany(keepAlive => keepAlive.Mappings));
                RegisterMappings<ChatMessagePacket>(link, ChatMessage.SelectMany(chatMessage => chatMessage.Mappings));
                RegisterMappings<SystemChatMessagePacket>(link, SystemChatMessage.SelectMany(systemChatMessage => systemChatMessage.Mappings));
                RegisterMappings<NbtDisconnectPacket>(link, PlayDisconnect.SelectMany(playDisconnect => playDisconnect.Mappings));
                break;
        }
    }

    private static void RegisterMappings<T>(ILink link, params IEnumerable<MinecraftPacketTransformationMapping> mappings) where T : IMinecraftPacket
    {
        var protocolVersion = link.Player.ProtocolVersion;
        link.PlayerChannel.MinecraftRegistries.PacketTransformationsSystem.All.RegisterTransformations<T>(protocolVersion, mappings);
        link.ServerChannel.MinecraftRegistries.PacketTransformationsSystem.All.RegisterTransformations<T>(protocolVersion, mappings);
    }
}
