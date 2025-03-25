using Void.Minecraft.Components.Text.Colors;

namespace Void.Minecraft.Components.Text.Properties;

public record Formatting(TextColor? Color = null, TextShadowColor? ShadowColor = null, string? Font = null, bool? IsBold = null, bool? IsItalic = null, bool? IsUnderlined = null, bool? IsStrikethrough = null, bool? IsObfuscated = null)
{
    public static Formatting Default { get; } = new();
}
