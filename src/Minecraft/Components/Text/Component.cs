using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;
using Void.Minecraft.Components.Text.Serializers;
using Void.Minecraft.Nbt;
using Void.Minecraft.Network;

namespace Void.Minecraft.Components.Text;

public record Component(IContent Content, Children Children, Formatting Formatting, Interactivity Interactivity)
{
    public static Component Default { get; } = new(new TextContent(string.Empty), Children.Default, Formatting.Default, Interactivity.Default);

    public static implicit operator Component?(string text) => string.IsNullOrWhiteSpace(text) ? null : DeserializeLegacy(text);

    public static Component ReadFrom(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
            return DeserializeLegacy(buffer.ReadString());
        else
            return DeserializeNbt(buffer.ReadTag(), protocolVersion);
    }

    public void WriteTo(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
            buffer.WriteString(SerializeLegacy());
        else
            buffer.WriteTag(SerializeNbt(protocolVersion));
    }

    public static Component DeserializeLegacy(string source, char prefix = '&')
    {
        return LegacyComponentSerializer.Deserialize(source, prefix);
    }

    public static Component DeserializeNbt(NbtTag tag, ProtocolVersion protocolVersion)
    {
        return NbtComponentSerializer.Deserialize(tag, protocolVersion);
    }

    public string SerializeLegacy(char prefix = '&')
    {
        return LegacyComponentSerializer.Serialize(this, prefix);
    }

    public NbtTag SerializeNbt(ProtocolVersion protocolVersion)
    {
        return NbtComponentSerializer.Serialize(this, protocolVersion);
    }
}