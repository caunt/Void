namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>
/// Represents literal text content in a Minecraft text component.
/// </summary>
/// <param name="Value">The text displayed by the component.</param>
public record TextContent(string Value) : IContent
{
    /// <summary>Gets the <c>text</c> content discriminator.</summary>
    public string Type => "text";
}
