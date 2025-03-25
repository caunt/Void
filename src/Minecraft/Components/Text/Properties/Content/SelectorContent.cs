namespace Void.Minecraft.Components.Text.Properties.Content;

public record SelectorContent(string Value, Component Separator) : IContent
{
    public string Type => "selector";
}
