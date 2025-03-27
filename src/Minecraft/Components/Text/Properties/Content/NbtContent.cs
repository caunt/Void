namespace Void.Minecraft.Components.Text.Properties.Content;

public record NbtContent(string Path, string? Source = null, bool? Interpret = null, Component? Separator = null, string? Block = null, string? Entity = null, string? Storage = null) : IContent
{
    public string Type => "nbt";
}
