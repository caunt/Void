using Nito.Disposables.Internals;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Components.Text.Colors;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Nbt.Serializers.String;
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
        if (!TryParse(value, out var node))
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
            node = Downgrade_v1_20_3_to_v1_20_2(node);

        if (from > ProtocolVersion.MINECRAFT_1_15_2 && to <= ProtocolVersion.MINECRAFT_1_15_2)
            node = Downgrade_v1_16_to_v1_15_2(node);

        return node;
    }

    private static JsonNode Upgrade(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        if (from <= ProtocolVersion.MINECRAFT_1_15_2 && to > ProtocolVersion.MINECRAFT_1_15_2)
            node = Upgrade_v1_15_2_to_v1_16(node);

        if (from <= ProtocolVersion.MINECRAFT_1_20_2 && to > ProtocolVersion.MINECRAFT_1_20_2)
            node = Upgrade_v1_20_2_to_v1_20_3(node);

        return node;
    }

    public static void Passthrough_v1_20_3_to_v1_20_2(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Downgrade_v1_20_3_to_v1_20_2(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }

    public static void Passthrough_v1_16_to_v1_15_2(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Downgrade_v1_16_to_v1_15_2(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }

    public static void Passthrough_v1_20_2_to_v1_20_3(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Upgrade_v1_20_2_to_v1_20_3(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }

    public static void Passthrough_v1_15_2_to_v1_16(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Upgrade_v1_15_2_to_v1_16(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }

    public static JsonNode Downgrade_v1_20_3_to_v1_20_2(JsonNode node)
    {
        Console.WriteLine("Json Downgrade_v1_20_3_to_v1_20_2 not supported");
        return node;
    }

    public static JsonNode Downgrade_v1_16_to_v1_15_2(JsonNode node)
    {
        if (node is JsonObject root)
        {
            if (root["color"] is { } colorTag)
            {
                var color = TextColor.FromString(colorTag.GetValue<string>());
                var downsampled = color.Downsample();

                root["color"] = downsampled.Name;
            }

            if (root["hoverEvent"] is JsonObject hoverEvent)
            {
                if (hoverEvent["contents"] is JsonObject contentsCompound)
                {
                    hoverEvent.Remove("contents");

                    if (contentsCompound["action"] is { } action)
                    {
                        if (action.GetValue<string>() is "show_text" or "show_achievement")
                        {
                            hoverEvent["value"] = contentsCompound;
                        }
                        else if (action.GetValue<string>() is "show_item" or "show_entity")
                        {
                            hoverEvent["value"] = contentsCompound.ToString();
                        }
                    }
                }
            }

            if (root["with"] is JsonArray with)
                root["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_16_to_v1_15_2(childNode)))]);

            if (root["extra"] is JsonArray extra)
                root["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_16_to_v1_15_2(childNode)))]);
        }

        return node;
    }

    public static JsonNode Upgrade_v1_20_2_to_v1_20_3(JsonNode node)
    {
        Console.WriteLine("Json Upgrade_v1_20_2_to_v1_20_3 not supported");
        return node;
    }

    public static JsonNode Upgrade_v1_15_2_to_v1_16(JsonNode node)
    {
        if (node is JsonObject root)
        {
            if (root["hoverEvent"] is JsonObject hoverEvent)
            {
                if (hoverEvent["value"] is { } value)
                {
                    hoverEvent.Remove("value");

                    if (hoverEvent["action"] is { } action)
                    {
                        var contents = (JsonNode?)null;

                        if (action.GetValue<string>() is "show_text" or "show_achievement")
                        {
                            contents = value;
                        }
                        else if (action.GetValue<string>() is "show_item" or "show_entity")
                        {
                            if (value["text"] is not { } text)
                                throw new NotSupportedException("SNBT text is not found");

                            var stringNbtText = text.GetValue<string>();
                            var nbtText = NbtStringSerializer.Deserialize(stringNbtText);
                            var jsonText = NbtJsonSerializer.Serialize(nbtText);

                            contents = jsonText;
                        }

                        hoverEvent["contents"] = contents;
                    }
                }
            }

            if (root["with"] is JsonArray with)
                root["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_15_2_to_v1_16(childNode)))]);

            if (root["extra"] is JsonArray extra)
                root["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_15_2_to_v1_16(childNode)))]);
        }

        return node;
    }

    private static bool TryParse(string value, [MaybeNullWhen(false)] out JsonNode node)
    {
        node = null;

        try
        {
            node = JsonNode.Parse(value);
        }
        catch
        {
            // Ignore, not a json
        }

        return node is not null;
    }
}
