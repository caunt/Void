namespace Void.Minecraft.Components.Text.Properties.Content;

/// <param name="Source">The optional source selector for the NBT data. When omitted, the component relies on one of <paramref name="Block"/>, <paramref name="Entity"/>, or <paramref name="Storage"/> to identify the data source.</param>
public record NbtContent(string Path, string? Source = null, bool? Interpret = null, Component? Separator = null, string? Block = null, string? Entity = null, string? Storage = null) : IContent
{
    public string Type => "nbt";
}
