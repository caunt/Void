using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class SystemChatMessagePacket : IMinecraftClientboundPacket<SystemChatMessagePacket>
{
    public static MinecraftPacketTransformationMapping[] Transformations { get; } = [
        // Not working since 1.20.2 (including)
        // TODO - upgrade packet here, not rely on protocolVersion below
        //
        // new(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.MINECRAFT_1_20_3, wrapper =>
        // {
        //     var componentJson = wrapper.Read<StringProperty>();
        //     var node = JsonNode.Parse(componentJson.AsPrimitive) ??
        //         throw new InvalidOperationException("Failed to parse component JSON");
        // 
        //     var component = Component.DeserializeJson(node);
        //     wrapper.Write(NamedNbtProperty.FromNbtTag(component.SerializeNbt()));
        // 
        //     wrapper.Passthrough<BoolProperty>();
        // }),
        // new(ProtocolVersion.MINECRAFT_1_20_3, ProtocolVersion.MINECRAFT_1_20_2, wrapper =>
        // {
        //     ComponentNbtTransformers.Passthrough_v1_20_3_to_v1_20_2(wrapper);
        //     wrapper.Passthrough<BoolProperty>();
        // }),

        // TODO - Required but will not work until upgrades above are done
        // new(ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_5, wrapper =>
        // {
        //     ComponentNbtTransformers.Passthrough_v1_21_4_to_v1_21_5(wrapper);
        //     wrapper.Passthrough<BoolProperty>();
        // }),
        // new(ProtocolVersion.MINECRAFT_1_21_5, ProtocolVersion.MINECRAFT_1_21_4, wrapper =>
        // {
        //     ComponentNbtTransformers.Passthrough_v1_21_5_to_v1_21_4(wrapper);
        //     wrapper.Passthrough<BoolProperty>();
        // })
    ];

    public required Component Message { get; set; }
    public required bool Overlay { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, protocolVersion);
        buffer.WriteBoolean(Overlay);
    }

    public static SystemChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var message = buffer.ReadComponent(protocolVersion);
        var overlay = buffer.ReadBoolean();

        return new SystemChatMessagePacket
        {
            Message = message,
            Overlay = overlay
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
