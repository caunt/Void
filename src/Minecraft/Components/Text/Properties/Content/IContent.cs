namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>Defines the value-producing payload of a Minecraft text component.</summary>
public interface IContent
{
    /// <summary>Gets the serialized component content discriminator.</summary>
    public string Type { get; }
}
