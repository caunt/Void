using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Network;

namespace Void.Minecraft.Components.Text.Serializers;

public static class ComponentJsonSerializer
{
    public static JsonNode Serialize(Component component, ProtocolVersion protocolVersion)
    {
        var tag = component.SerializeNbt(protocolVersion);
        return NbtJsonSerializer.Serialize(tag);
    }

    public static Component Deserialize(string value, ProtocolVersion protocolVersion)
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
            return Deserialize(node: value, protocolVersion);
        else
            return Deserialize(node, protocolVersion);
    }

    public static Component Deserialize(JsonNode node, ProtocolVersion protocolVersion)
    {
        var tag = NbtJsonSerializer.Deserialize(node);
        return Component.DeserializeNbt(tag, protocolVersion);
    }
}
