namespace Void.Minecraft.Components.Text.Properties.Content;

public record Text(string Value) : IContent
{
    public string Type => "text";
}
