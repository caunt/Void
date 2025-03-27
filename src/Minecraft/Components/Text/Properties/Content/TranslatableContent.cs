using System.Collections.Generic;

namespace Void.Minecraft.Components.Text.Properties.Content;

public record TranslatableContent(string Translate, string? Fallback = null, IEnumerable<Component>? With = null) : IContent
{
    public string Type => "translatable";
}
