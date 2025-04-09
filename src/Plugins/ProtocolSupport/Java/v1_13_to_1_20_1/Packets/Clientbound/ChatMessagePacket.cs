using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

public class ChatMessagePacket : IMinecraftClientboundPacket<ChatMessagePacket>
{
    public static MinecraftPacketTransformationMapping[] Transformations { get; } = [
        new(ProtocolVersion.MINECRAFT_1_16, ProtocolVersion.MINECRAFT_1_15_2, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_16_to_v1_15_2(wrapper);
            wrapper.Passthrough<ByteProperty>();
            _ = wrapper.Read<UuidProperty>();
        }),
        new(ProtocolVersion.MINECRAFT_1_15_2, ProtocolVersion.MINECRAFT_1_16, wrapper =>
        {
            ComponentJsonTransformers.Passthrough_v1_15_2_to_v1_16(wrapper);
            wrapper.Passthrough<ByteProperty>();
            wrapper.Write(UuidProperty.Empty);
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
