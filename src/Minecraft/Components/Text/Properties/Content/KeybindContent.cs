namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>Represents text resolved from a client key-binding identifier.</summary>
/// <param name="Value">The key-binding translation identifier.</param>
public record KeybindContent(string Value) : IContent
{
    /// <summary>Gets the <c>keybind</c> content discriminator.</summary>
    public string Type => "keybind";
}
