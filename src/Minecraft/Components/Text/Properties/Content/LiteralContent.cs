using Void.Minecraft.Nbt;

namespace Void.Minecraft.Components.Text.Properties.Content;

public record LiteralContent(NbtTag Value) : IContent
{
    public string Type => "literal";
}
