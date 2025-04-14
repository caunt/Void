using System.Text.Json;
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
        var node = (JsonNode?)null;

        try
        {
            node = JsonNode.Parse(value);
        }
        catch (JsonException)
        {
            // ignore, not a json
        }

        if (node is null)
            return Deserialize(value);
        else
            return Deserialize(node);
    }

    public static Component Deserialize(JsonNode node)
    {
        var tag = NbtJsonSerializer.Deserialize(node);
        return Component.DeserializeNbt(tag);
    }
}
