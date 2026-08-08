using Void.Minecraft.Components.Text.Colors;

namespace Void.Minecraft.Components.Text.Properties;

/// <summary>Describes optional visual formatting inherited or applied by a text component.</summary>
/// <param name="Color">The foreground color, or <see langword="null"/> when unspecified.</param>
/// <param name="ShadowColor">The shadow color, or <see langword="null"/> when unspecified.</param>
/// <param name="Font">The font resource location, or <see langword="null"/> when unspecified.</param>
/// <param name="IsBold">Whether bold text is enabled, or <see langword="null"/> when unspecified.</param>
/// <param name="IsItalic">Whether italic text is enabled, or <see langword="null"/> when unspecified.</param>
/// <param name="IsUnderlined">Whether underlining is enabled, or <see langword="null"/> when unspecified.</param>
/// <param name="IsStrikethrough">Whether strikethrough is enabled, or <see langword="null"/> when unspecified.</param>
/// <param name="IsObfuscated">Whether obfuscated text is enabled, or <see langword="null"/> when unspecified.</param>
public record Formatting(TextColor? Color = null, TextShadowColor? ShadowColor = null, string? Font = null, bool? IsBold = null, bool? IsItalic = null, bool? IsUnderlined = null, bool? IsStrikethrough = null, bool? IsObfuscated = null)
{
    /// <summary>Gets formatting with every property unspecified.</summary>
    public static Formatting Default { get; } = new();
}
