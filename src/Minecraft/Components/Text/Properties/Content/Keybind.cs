namespace Void.Minecraft.Components.Text.Properties.Content;

public record Keybind(string Value) : IContent
{
    public string Type => "keybind";
}
