using Void.Common.Network;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Services.Transformations;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Transformations;

public class TransformationService : AbstractTransformationService
{
    [Subscribe(PostOrder.First)]
    public static void OnPhaseChangedEvent(PhaseChangedEvent @event)
    {
        if (@event is not { Side: Side.Client, Phase: Phase.Play })
            return;

        @event.Player.RegisterSystemTransformations<SystemChatMessagePacket>([
            // new(ProtocolVersion.MINECRAFT_1_20, ProtocolVersion.MINECRAFT_1_19, wrapper =>
            // {
            //     logger.LogInformation("Transforming downgrade {PacketType}", typeof(SystemChatMessagePacket));
            // 
            //     var property = wrapper.Read<NbtProperty>();
            //     property = ComponentNbtTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_2);
            //     wrapper.Write(property);
            // 
            //     wrapper.Passthrough<BoolProperty>();
            // }),
            // new(ProtocolVersion.MINECRAFT_1_19, ProtocolVersion.MINECRAFT_1_20, wrapper =>
            // {
            //     logger.LogInformation("Transforming upgrade {PacketType}", typeof(SystemChatMessagePacket));
            // 
            //     var property = wrapper.Read<NbtProperty>();
            //     property = ComponentNbtTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_2);
            //     wrapper.Write(property);
            // 
            //     wrapper.Passthrough<BoolProperty>();
            // })
        ]);

        @event.Player.RegisterSystemTransformations<ChatMessagePacket>(ChatMessagePacket.Transformations);
    }
}
