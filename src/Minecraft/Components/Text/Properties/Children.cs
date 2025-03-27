using System.Collections.Generic;

namespace Void.Minecraft.Components.Text.Properties;

public record Children(IEnumerable<Component> Extra)
{
    public static Children Default { get; } = new([]);
}
