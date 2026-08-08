using Void.Minecraft.Nbt;

namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>Represents a component whose literal payload is an arbitrary NBT tag.</summary>
/// <param name="Value">The literal NBT value.</param>
public record LiteralContent(NbtTag Value) : IContent
{
    /// <summary>Gets the <c>literal</c> content discriminator.</summary>
    public string Type => "literal";
}
