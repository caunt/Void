using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Minecraft.Components.Text.Transformers;

public static class ComponentJsonTransformers
{
    public static StringProperty Apply(StringProperty property, ProtocolVersion from, ProtocolVersion to)
    {
        return StringProperty.FromPrimitive(Apply(property.AsPrimitive, from, to));
    }

    public static string Apply(string value, ProtocolVersion from, ProtocolVersion to)
    {
        var node = (JsonNode?)null;

        try
        {
            node = JsonNode.Parse(value);
        }
        catch
        {
            // Ignore, not a json
        }

        if (node is null)
            return value;

        return Apply(node, from, to).ToString();
    }

    public static JsonNode Apply(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        if (node.GetValueKind() is not JsonValueKind.Object)
            return node;

        var type = from > to ? TransformationType.Downgrade : TransformationType.Upgrade;

        return type switch
        {
            TransformationType.Downgrade => Downgrade(node, from, to),
            TransformationType.Upgrade => Upgrade(node, from, to),
            _ => node
        };
    }

    private static JsonNode Downgrade(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        if (from > ProtocolVersion.MINECRAFT_1_20_2 && to <= ProtocolVersion.MINECRAFT_1_20_2)
            node = Downgrade_v1_20_3_to_v1_20_2(node, from, to);

        if (from > ProtocolVersion.MINECRAFT_1_15_2 && to <= ProtocolVersion.MINECRAFT_1_15_2)
            node = Downgrade_v1_16_to_v1_15_2(node, from, to);

        return node;
    }

    private static JsonNode Upgrade(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        if (from <= ProtocolVersion.MINECRAFT_1_15_2 && to > ProtocolVersion.MINECRAFT_1_15_2)
            node = Upgrade_v1_15_2_to_v1_16(node, from, to);

        if (from <= ProtocolVersion.MINECRAFT_1_20_2 && to > ProtocolVersion.MINECRAFT_1_20_2)
            node = Upgrade_v1_20_2_to_v1_20_3(node, from, to);

        return node;
    }

    private static JsonNode Downgrade_v1_20_3_to_v1_20_2(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        Console.WriteLine("Json Downgrade_v1_20_3_to_v1_20_2 not supported");
        return node;
    }

    private static JsonNode Downgrade_v1_16_to_v1_15_2(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        Console.WriteLine("Json Downgrade_v1_16_to_v1_15_2 not supported");
        return node;
    }

    private static JsonNode Upgrade_v1_20_2_to_v1_20_3(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        Console.WriteLine("Json Upgrade_v1_20_2_to_v1_20_3 not supported");
        return node;
    }

    private static JsonNode Upgrade_v1_15_2_to_v1_16(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        Console.WriteLine("Json Upgrade_v1_15_2_to_v1_16 not supported");
        return node;
    }
}
