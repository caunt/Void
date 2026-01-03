using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Components.Text.Transformers;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

public class SystemChatMessagePacket : IMinecraftClientboundPacket<SystemChatMessagePacket>
{
    public static MinecraftPacketTransformationMapping[] Transformations { get; } = [
        new(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.MINECRAFT_1_20_3, wrapper =>
        {
            var jsonStringProperty = wrapper.Read<StringProperty>();
            var componentNbt = ComponentNbtTransformers.Upgrade_v1_20_2_to_v1_20_3(NbtJsonSerializer.Deserialize(jsonStringProperty.AsJsonNode));

            wrapper.Write(NbtProperty.FromNbtTag(componentNbt));
            wrapper.Passthrough<BoolProperty>();
        }),
        new(ProtocolVersion.MINECRAFT_1_20_3, ProtocolVersion.MINECRAFT_1_20_2, wrapper =>
        {
            var namedNbtProperty = wrapper.Read<NbtProperty>();
            var componentNbt = ComponentNbtTransformers.Downgrade_v1_20_3_to_v1_20_2(namedNbtProperty.AsNbtTag);

            wrapper.Write(StringProperty.FromJsonNode(NbtJsonSerializer.Serialize(componentNbt)));
            wrapper.Passthrough<BoolProperty>();
        }),

        new(ProtocolVersion.MINECRAFT_1_21_4, ProtocolVersion.MINECRAFT_1_21_5, wrapper =>
        {
            var property = wrapper.Read<NbtProperty>();
            var tag = ComponentNbtTransformers.Upgrade_v1_21_4_to_v1_21_5(property.AsNbtTag);

            wrapper.Write(NbtProperty.FromNbtTag(tag));
            wrapper.Passthrough<BoolProperty>();
        }),
        new(ProtocolVersion.MINECRAFT_1_21_5, ProtocolVersion.MINECRAFT_1_21_4, wrapper =>
        {
            var property = wrapper.Read<NbtProperty>();
            var tag = ComponentNbtTransformers.Downgrade_v1_21_5_to_v1_21_4(property.AsNbtTag);

            wrapper.Write(NbtProperty.FromNbtTag(tag));
            wrapper.Passthrough<BoolProperty>();
        })
    ];

    public required Component Message { get; set; }
    public required bool Overlay { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteComponent(Message, writeName: false);
        buffer.WriteBoolean(Overlay);
    }

    public static SystemChatMessagePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var message = buffer.ReadComponent();
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
