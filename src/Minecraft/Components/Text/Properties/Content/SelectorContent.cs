namespace Void.Minecraft.Components.Text.Properties.Content;

public record SelectorContent(string Value, Component? Separator = null) : IContent
{
    public string Type => "selector";
}
