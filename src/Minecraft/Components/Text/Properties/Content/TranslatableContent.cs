using System.Collections.Generic;

namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>
/// Represents a text component whose displayed value is resolved from a client translation key.
/// </summary>
/// <param name="Translate">The translation key written to the component's <c>translate</c> field.</param>
/// <param name="Fallback">Optional literal text used when the client cannot resolve <paramref name="Translate"/>.</param>
/// <param name="With">Optional <see cref="Component"/> arguments substituted into the translated pattern.</param>
/// <remarks>
/// Serializers omit <paramref name="Fallback"/> and <paramref name="With"/> when they are <see langword="null"/>.
/// </remarks>
public record TranslatableContent(string Translate, string? Fallback = null, IEnumerable<Component>? With = null) : IContent
{
    public string Type => "translatable";
}
