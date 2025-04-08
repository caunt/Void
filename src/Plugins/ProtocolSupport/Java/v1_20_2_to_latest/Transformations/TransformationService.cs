using Microsoft.Extensions.Logging;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Plugins.Common.Services.Transformations;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Transformations;

public class TransformationService(ILogger<TransformationService> logger) : AbstractTransformationService
{
    [Subscribe]
    public void OnLinkStarted(LinkStartedEvent @event)
    {
        @event.Link.RegisterTransformations<SystemChatMessagePacket>([
            new(ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_2, wrapper =>
            {
                wrapper.Passthrough<NbtProperty>(); // component
                wrapper.Passthrough<BoolProperty>(); // overlay
                logger.LogInformation("1.21.4 => 1.21.2");
            })
        ]);
    }
}
