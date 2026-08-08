using System.Collections.Generic;

namespace Void.Minecraft.Components.Text.Properties;

/// <summary>Contains the child components appended to a text component.</summary>
/// <param name="Extra">The ordered child-component sequence.</param>
public record Children(IEnumerable<Component> Extra)
{
    /// <summary>Gets an empty child-component collection.</summary>
    public static Children Default { get; } = new([]);
}
