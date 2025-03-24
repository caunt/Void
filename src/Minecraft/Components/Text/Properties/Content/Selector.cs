namespace Void.Minecraft.Components.Text.Properties.Content;

public record Selector(string Value, Component Separator) : IContent
{
    public string Type => "selector";
}
