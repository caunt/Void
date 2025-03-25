using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;
using Void.Minecraft.Components.Text.Serializers;

namespace Void.Minecraft.Components.Text;

public record Component(IContent Content, Children Children, Formatting Formatting, Interactivity Interactivity)
{
    public static Component Default { get; } = new(new TextContent(string.Empty), Children.Default, Formatting.Default, Interactivity.Default);

    public static Component DeserializeLegacy(string source, char prefix = '&')
    {
        return LegacyComponentSerializer.Deserialize(source, prefix);
    }

    public string SerializeLegacy(char prefix = '&')
    {
        return LegacyComponentSerializer.Serialize(this, prefix);
    }
}