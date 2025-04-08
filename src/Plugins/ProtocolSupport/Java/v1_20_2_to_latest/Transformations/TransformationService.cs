using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Plugins.Common.Services.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Transformations;

public class TransformationService : AbstractTransformationService
{
    [Subscribe]
    public static void OnLinkStarted(LinkStartedEvent @event)
    {
        // @event.Link.RegisterTransformations<SystemChatMessagePacket>([
        //     new(ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_2, wrapper =>
        //     {
        //         var property = wrapper.Read<NbtProperty>();
        //         property = ComponentNbtTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_2);
        //         wrapper.Write(property);
        // 
        //         wrapper.Passthrough<BoolProperty>();
        //     })
        // ]);
    }
}
