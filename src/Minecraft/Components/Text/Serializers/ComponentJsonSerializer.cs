using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Network;

namespace Void.Minecraft.Components.Text.Serializers;

public static class ComponentJsonSerializer
{
    public static JsonNode Serialize(Component component, ProtocolVersion protocolVersion)
    {
        var tag = component.SerializeNbt(protocolVersion);
        return JsonNbtSerializer.Serialize(tag);
    }

    public static Component Deserialize(JsonNode node, ProtocolVersion protocolVersion)
    {
        var tag = JsonNbtSerializer.Deserialize(node);
        return Component.DeserializeNbt(tag, protocolVersion);
    }
}
