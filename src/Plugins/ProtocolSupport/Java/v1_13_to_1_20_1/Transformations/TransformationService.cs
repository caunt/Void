using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Plugins.Common.Services.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Transformations;

public class TransformationService(ILogger<TransformationService> logger) : AbstractTransformationService
{
    [Subscribe]
    public void OnLinkStarted(LinkStartedEvent @event)
    {
        // @event.Link.RegisterTransformations<SystemChatMessagePacket>([
        //     new(ProtocolVersion.MINECRAFT_1_20, ProtocolVersion.MINECRAFT_1_19, wrapper =>
        //     {
        //         logger.LogInformation("Transforming downgrade {PacketType}", typeof(SystemChatMessagePacket));
        // 
        //         var property = wrapper.Read<NbtProperty>();
        //         property = ComponentNbtTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_2);
        //         wrapper.Write(property);
        // 
        //         wrapper.Passthrough<BoolProperty>();
        //     }),
        //     new(ProtocolVersion.MINECRAFT_1_19, ProtocolVersion.MINECRAFT_1_20, wrapper =>
        //     {
        //         logger.LogInformation("Transforming upgrade {PacketType}", typeof(SystemChatMessagePacket));
        // 
        //         var property = wrapper.Read<NbtProperty>();
        //         property = ComponentNbtTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_2);
        //         wrapper.Write(property);
        // 
        //         wrapper.Passthrough<BoolProperty>();
        //     })
        // ]);
        // 
        // @event.Link.RegisterTransformations<ChatMessagePacket>([
        //     new(ProtocolVersion.MINECRAFT_1_18_2, ProtocolVersion.MINECRAFT_1_16, wrapper =>
        //     {
        //         logger.LogInformation("Transforming downgrade {PacketType} 1", typeof(ChatMessagePacket));
        // 
        //         var property = wrapper.Read<StringProperty>();
        //         property = ComponentJsonTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_18_2, ProtocolVersion.MINECRAFT_1_16);
        //         wrapper.Write(property);
        // 
        //         wrapper.Passthrough<ByteProperty>();
        //     }),
        //     new(ProtocolVersion.MINECRAFT_1_16, ProtocolVersion.MINECRAFT_1_18_2, wrapper =>
        //     {
        //         logger.LogInformation("Transforming upgrade {PacketType} 2", typeof(ChatMessagePacket));
        // 
        //         var property = wrapper.Read<StringProperty>();
        //         property = ComponentJsonTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_16, ProtocolVersion.MINECRAFT_1_18_2);
        //         wrapper.Write(property);
        // 
        //         wrapper.Passthrough<ByteProperty>();
        //     }),
        // 
        // 
        // 
        //     new(ProtocolVersion.MINECRAFT_1_16, ProtocolVersion.MINECRAFT_1_13, wrapper =>
        //     {
        //         logger.LogInformation("Transforming downgrade {PacketType} 3", typeof(ChatMessagePacket));
        // 
        //         var property = wrapper.Read<StringProperty>();
        //         property = ComponentJsonTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_16, ProtocolVersion.MINECRAFT_1_13);
        //         wrapper.Write(property);
        // 
        //         wrapper.Passthrough<ByteProperty>();
        //         // wrapper.Write(UuidProperty.FromUuid(new Uuid(Guid.Empty)));
        //     }),
        //     new(ProtocolVersion.MINECRAFT_1_13, ProtocolVersion.MINECRAFT_1_16, wrapper =>
        //     {
        //         logger.LogInformation("Transforming upgrade {PacketType} 4", typeof(ChatMessagePacket));
        // 
        //         var property = wrapper.Read<StringProperty>();
        //         property = ComponentJsonTransformers.Apply(property, ProtocolVersion.MINECRAFT_1_13, ProtocolVersion.MINECRAFT_1_16);
        //         wrapper.Write(property);
        // 
        //         wrapper.Passthrough<ByteProperty>();
        //         // wrapper.Write(UuidProperty.FromUuid(new Uuid(Guid.Empty)));
        //     })
        // ]);
    }
}
