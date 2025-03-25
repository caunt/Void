namespace Void.Minecraft.Components.Text.Properties.Content;

public record TextContent(string Value) : IContent
{
    public string Type => "text";
}
