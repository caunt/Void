﻿using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Services.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Transformations;

public class TransformationService : AbstractTransformationService
{
    [Subscribe(PostOrder.First)]
    public static void OnPhaseChangedEvent(PhaseChangedEvent @event)
    {
        if (!Plugin.SupportedVersions.Contains(@event.Player.ProtocolVersion))
            return;

        if (@event is not { Side: Side.Client, Phase: Phase.Play })
            return;

        @event.Player.RegisterSystemTransformations<ChatMessagePacket>(ChatMessagePacket.Transformations);
    }
}
