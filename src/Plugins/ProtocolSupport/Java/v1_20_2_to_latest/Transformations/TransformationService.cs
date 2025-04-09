using Void.Minecraft.Events;
using Void.Proxy.Api.Events;
using Void.Proxy.Plugins.Common.Services.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Transformations;

public class TransformationService : AbstractTransformationService
{
    [Subscribe]
    public static void OnPhaseChangedEvent(PhaseChangedEvent @event)
    {
        if (!Plugin.SupportedVersions.Contains(@event.Player.ProtocolVersion))
            return;
    }
}
