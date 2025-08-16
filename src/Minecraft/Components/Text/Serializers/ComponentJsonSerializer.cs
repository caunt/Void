using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json;

namespace Void.Minecraft.Components.Text.Serializers;

public static class ComponentJsonSerializer
{
    public static JsonNode Serialize(Component component)
    {
        var tag = component.SerializeNbt();
        return NbtJsonSerializer.Serialize(tag);
    }

    public static Component Deserialize(string value)
    {
        var node = JsonNode.Parse(value);
        return node is null ? Component.Default : Deserialize(node);
    }

    public static Component Deserialize(JsonNode node)
    {
        var tag = NbtJsonSerializer.Deserialize(node);
        return Component.DeserializeNbt(tag);
    }
}
