using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Extensions;
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

    public static void Register(IPlayer player)
    {
        player.RegisterSystemTransformations<KeepAliveRequestPacket>(KeepAlive.SelectMany(keepAlive => keepAlive.Mappings));
        player.RegisterSystemTransformations<KeepAliveResponsePacket>(KeepAlive.SelectMany(keepAlive => keepAlive.Mappings));
        player.RegisterSystemTransformations<ChatMessagePacket>(ChatMessage.SelectMany(chatMessage => chatMessage.Mappings));
        player.RegisterSystemTransformations<SystemChatMessagePacket>(SystemChatMessage.SelectMany(systemChatMessage => systemChatMessage.Mappings));
    }
}
