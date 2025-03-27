using System.Text.Json.Nodes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Components.Text.Serializers;

public static class JsonComponentSerializer
{
    public static JsonNode Serialize(Component component, ProtocolVersion protocolVersion)
    {
        // TODO
        return LegacyComponentSerializer.Serialize(component);
    }

    public static Component Deserialize(JsonNode node, ProtocolVersion protocolVersion)
    {
        // TODO
        return LegacyComponentSerializer.Deserialize(node.GetValue<string>());
    }
}
