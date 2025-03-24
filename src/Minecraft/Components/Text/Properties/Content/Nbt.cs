namespace Void.Minecraft.Components.Text.Properties.Content;

public record Nbt(string Source, string Path, bool Interpret, Component Separator, string Block, string Entity, string Storage) : IContent
{
    public string Type => "nbt";
}
