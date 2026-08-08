namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>Represents text read from an NBT path in a block, entity, or command-storage source.</summary>
/// <param name="Path">The NBT path expression evaluated against the source.</param>
/// <param name="Source">An optional generic source value retained by the model.</param>
/// <param name="Interpret">Whether selected strings are interpreted as text components.</param>
/// <param name="Separator">The component inserted between multiple selected values, or <see langword="null"/>.</param>
/// <param name="Block">The block position source, or <see langword="null"/>.</param>
/// <param name="Entity">The entity selector source, or <see langword="null"/>.</param>
/// <param name="Storage">The command-storage resource location, or <see langword="null"/>.</param>
public record NbtContent(string Path, string? Source = null, bool? Interpret = null, Component? Separator = null, string? Block = null, string? Entity = null, string? Storage = null) : IContent
{
    /// <summary>Gets the <c>nbt</c> content discriminator.</summary>
    public string Type => "nbt";
}
