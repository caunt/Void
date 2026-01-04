using Void.Proxy.Api.Players;
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
    public static BaseTransformations[] All { get; } = [
        new KeepAliveTransformations1_8(),
        new KeepAliveTransformations1_12_2(),

        new ChatMessageTransformations1_8(),
        new ChatMessageTransformations1_9(),
        new ChatMessageTransformations1_12(),
        new ChatMessageTransformations1_16(),

        new SystemChatMessageTransformations1_20_3(),
        new SystemChatMessageTransformations1_21_5()
    ];

    public static void Register(IPlayer player)
    {
        foreach (var transformation in All)
            transformation.Register(player);
    }
}
