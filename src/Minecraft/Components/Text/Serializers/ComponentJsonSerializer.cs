using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json;

namespace Void.Minecraft.Components.Text.Serializers;

/// <summary>Converts Minecraft text components to and from their JSON representation.</summary>
public static class ComponentJsonSerializer
{
    /// <summary>Serializes a component through its NBT representation.</summary>
    /// <param name="component">The component to serialize.</param>
    /// <returns>The JSON node representing the component.</returns>
    public static JsonNode Serialize(Component component)
    {
        var tag = component.SerializeNbt();
        return NbtJsonSerializer.Serialize(tag);
    }

    /// <summary>Parses and deserializes a JSON component string.</summary>
    /// <param name="value">The JSON text.</param>
    /// <returns>The deserialized component, or <see cref="Component.Default"/> when parsing produces no root node.</returns>
    /// <exception cref="System.Text.Json.JsonException"><paramref name="value"/> is not valid JSON.</exception>
    public static Component Deserialize(string value)
    {
        var node = JsonNode.Parse(value);
        return node is null ? Component.Default : Deserialize(node);
    }

    /// <summary>Deserializes a component from a JSON node through the NBT serializer.</summary>
    /// <param name="node">The JSON representation.</param>
    /// <returns>The deserialized component.</returns>
    public static Component Deserialize(JsonNode node)
    {
        var tag = NbtJsonSerializer.Deserialize(node);
        return Component.DeserializeNbt(tag);
    }
}
