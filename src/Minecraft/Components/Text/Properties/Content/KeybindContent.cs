namespace Void.Minecraft.Components.Text.Properties.Content;

public record KeybindContent(string Value) : IContent
{
    public string Type => "keybind";
}
