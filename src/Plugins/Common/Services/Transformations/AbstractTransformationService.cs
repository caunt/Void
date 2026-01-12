using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations;

namespace Void.Proxy.Plugins.Common.Services.Transformations;

public abstract class AbstractTransformationService : IPluginCommonService
{

    [Subscribe(PostOrder.First)]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event is not { Side: Side.Server, Phase: Phase.Play })
            return;

        NetworkTransformations.Register(@event.Link, @event.Player);
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion protocolVersion);
}
