using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

public class ChatMessagePacket : IMinecraftClientboundPacket<ChatMessagePacket>
{
    public static MinecraftPacketTransformationMapping[] Transformations { get; } = [
        new(ProtocolVersion.MINECRAFT_1_8, ProtocolVersion.MINECRAFT_1_7_6, wrapper =>
        {
            wrapper.Passthrough<StringProperty>();
            _ = wrapper.Read<ByteProperty>();
        }),
        new(ProtocolVersion.MINECRAFT_1_7_6, ProtocolVersion.MINECRAFT_1_8, wrapper =>
        {
            wrapper.Passthrough<StringProperty>();
            wrapper.Write(ByteProperty.FromPrimitive(1));
        }),

        new(ProtocolVersion.MINECRAFT_1_9, ProtocolVersion.MINECRAFT_1_8, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_9_to_v1_8(wrapper);
            wrapper.Passthrough<ByteProperty>();
        }),
        new(ProtocolVersion.MINECRAFT_1_8, ProtocolVersion.MINECRAFT_1_9, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_8_to_v1_9(wrapper);
            wrapper.Passthrough<ByteProperty>();
        }),

        new(ProtocolVersion.MINECRAFT_1_11_1, ProtocolVersion.MINECRAFT_1_12, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_11_1_to_v1_12(wrapper);
            wrapper.Passthrough<ByteProperty>();
        }),
        new(ProtocolVersion.MINECRAFT_1_12, ProtocolVersion.MINECRAFT_1_11_1, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_12_to_v1_11_1(wrapper);
            wrapper.Passthrough<ByteProperty>();
        }),

        new(ProtocolVersion.MINECRAFT_1_15_2, ProtocolVersion.MINECRAFT_1_16, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_15_2_to_v1_16(wrapper);
            wrapper.Passthrough<ByteProperty>();
            wrapper.Write(UuidProperty.Empty);
        }),
        new(ProtocolVersion.MINECRAFT_1_16, ProtocolVersion.MINECRAFT_1_15_2, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_16_to_v1_15_2(wrapper);
            wrapper.Passthrough<ByteProperty>();
            _ = wrapper.Read<UuidProperty>();
        }),

        new(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.MINECRAFT_1_20_3, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_20_2_to_v1_20_3(wrapper);
            wrapper.Passthrough<ByteProperty>();
            wrapper.Passthrough<UuidProperty>();
        }),
        new(ProtocolVersion.MINECRAFT_1_20_3, ProtocolVersion.MINECRAFT_1_20_2, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_20_3_to_v1_20_2(wrapper);
            wrapper.Passthrough<ByteProperty>();
            wrapper.Passthrough<UuidProperty>();
        })
    ];

    public required Component Message { get; set; }
    public byte Position { get; set; }
    public Uuid Sender { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, protocolVersion);
        buffer.WriteUnsignedByte(Position);
        buffer.WriteUuid(Sender);
    }

    public static ChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var message = buffer.ReadComponent(protocolVersion);
        var position = buffer.ReadUnsignedByte();
        var sender = buffer.ReadUuid();

        return new ChatMessagePacket
        {
            Message = message,
            Position = position,
            Sender = sender
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
