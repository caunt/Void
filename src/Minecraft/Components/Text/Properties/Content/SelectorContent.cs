namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>
/// Represents a text component whose displayed value is resolved from an entity selector.
/// </summary>
/// <param name="Value">The Minecraft entity selector pattern written to the component's <c>selector</c> field.</param>
/// <param name="Separator">Optional component inserted between multiple resolved selector results; serializers omit it when it is <see langword="null"/>.</param>
public record SelectorContent(string Value, Component? Separator = null) : IContent
{
    public string Type => "selector";
}
