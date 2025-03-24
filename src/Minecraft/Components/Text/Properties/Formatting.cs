using Void.Minecraft.Components.Text.Colors;

namespace Void.Minecraft.Components.Text.Properties;

public record Formatting(TextColor Color, TextShadowColor ShadowColor, string Font, bool IsBold, bool IsItalic, bool IsUnderlined, bool IsStrikethrough, bool IsObfuscated);
