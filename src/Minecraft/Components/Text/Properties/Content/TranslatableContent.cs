using System.Collections.Generic;

namespace Void.Minecraft.Components.Text.Properties.Content;

public record TranslatableContent(string Translate, string Fallback, List<Component> With) : IContent
{
    public string Type => "translatable";
}
