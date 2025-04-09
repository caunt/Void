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
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Transformers;

public static class ComponentJsonTransformers
{
    private const string ShowAchievementMarker = "!1.11.1=>1.12!";

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
        // Replace "show_entity" contents "id" int array value to string
        if (node is JsonObject rootObject)
        {
            if (rootObject["hoverEvent"] is JsonObject hoverEvent)
            {
                if (hoverEvent["contents"] is JsonObject contentsObject)
                {
                    if (contentsObject["action"] is { } action)
                    {
                        if (action.GetValue<string>() is "show_entity")
                        {
                            if (contentsObject["id"] is JsonArray idIntArray)
                                contentsObject["id"] = Uuid.Parse([.. idIntArray.Select(node => node?.ToString() ?? throw new JsonException("Null entity id int element found")).Select(int.Parse)]).ToString();
                            else if (contentsObject["id"]?.GetValueKind() is not JsonValueKind.String)
                                throw new NotSupportedException($"Non-string id value found: {contentsObject["id"]}");
                        }
                    }
                }
            }
        }

        return node;
    }

    public static JsonNode Downgrade_v1_16_to_v1_15_2(JsonNode node)
    {
        if (node is JsonObject rootObject)
        {
            // Downsample colors to list of compatible
            if (rootObject["color"] is { } colorTag)
            {
                var color = TextColor.FromString(colorTag.GetValue<string>());
                var downsampled = color.Downsample();

                rootObject["color"] = downsampled.Name;
            }

            // Replace the "contents" field with "value" field
            if (rootObject["hoverEvent"] is JsonObject hoverEvent)
            {
                if (hoverEvent["contents"] is JsonObject contentsObject)
                {
                    hoverEvent.Remove("contents");

                    if (contentsObject["action"] is { } action)
                    {
                        if (action.GetValue<string>() is "show_text" or "show_achievement")
                        {
                            hoverEvent["value"] = contentsObject;
                        }
                        else if (action.GetValue<string>() is "show_item" or "show_entity")
                        {
                            hoverEvent["value"] = contentsObject.ToString();
                        }
                    }
                }
            }

            // Replace recursive text components
            if (rootObject["with"] is JsonArray with)
                rootObject["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_16_to_v1_15_2(childNode)))]);

            if (rootObject["extra"] is JsonArray extra)
                rootObject["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_16_to_v1_15_2(childNode)))]);
        }

        // De-compact text component
        if (node.GetValueKind() is JsonValueKind.String)
            node = new JsonObject { ["text"] = JsonSerializer.SerializeToNode(node) };

        return node;
    }

    public static JsonNode Downgrade_v1_12_to_v1_11_1(JsonNode node)
    {
        if (node is JsonObject rootObject)
        {
            // Replace the "show_text" action back to "show_achievement"
            if (rootObject["hoverEvent"] is JsonObject hoverEvent)
            {
                if (hoverEvent["value"] is { } value)
                {
                    if (hoverEvent["action"] is { } action)
                    {
                        if (value.GetValueKind() is JsonValueKind.String)
                        {
                            var valueString = value.GetValue<string>();

                            if (valueString.StartsWith(ShowAchievementMarker))
                            {
                                if (action.GetValue<string>() is "show_text")
                                {
                                    hoverEvent["action"] = "show_achievement";
                                    hoverEvent["value"] = new JsonObject { ["text"] = valueString[ShowAchievementMarker.Length..] };
                                }
                            }
                        }
                    }
                }
            }

            // Replace recursive text components
            if (rootObject["with"] is JsonArray with)
                rootObject["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_16_to_v1_15_2(childNode)))]);

            if (rootObject["extra"] is JsonArray extra)
                rootObject["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_16_to_v1_15_2(childNode)))]);
        }

        return node;
    }

    public static JsonNode Upgrade_v1_20_2_to_v1_20_3(JsonNode node)
    {
        return node;
    }

    public static JsonNode Upgrade_v1_15_2_to_v1_16(JsonNode node)
    {
        if (node is JsonObject root)
        {
            // Replace the "value" field with "contents" field
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

            // Replace recursive text components
            if (root["with"] is JsonArray with)
                root["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_15_2_to_v1_16(childNode)))]);

            if (root["extra"] is JsonArray extra)
                root["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_15_2_to_v1_16(childNode)))]);
        }

        return node;
    }

    public static JsonNode Upgrade_v1_11_1_to_v1_12(JsonNode node)
    {
        if (node is JsonObject root)
        {
            // Replace the "show_achievement" action with "show_text"
            if (root["hoverEvent"] is JsonObject hoverEvent)
            {
                if (hoverEvent["value"] is { } value)
                {
                    if (hoverEvent["action"] is { } action)
                    {
                        if (action.GetValue<string>() is "show_achievement")
                        {
                            hoverEvent["action"] = "show_text";

                            if (value["text"] is not { } text)
                                throw new NotSupportedException($"Text value not found: {value}");

                            hoverEvent["value"] = ShowAchievementMarker + text.GetValue<string>();
                        }
                    }
                }
            }

            // Replace recursive text components
            if (root["with"] is JsonArray with)
                root["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_11_1_to_v1_12(childNode)))]);

            if (root["extra"] is JsonArray extra)
                root["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_11_1_to_v1_12(childNode)))]);
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
