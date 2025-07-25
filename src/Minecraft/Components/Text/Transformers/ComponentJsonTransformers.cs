﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Nito.Disposables.Internals;
using Void.Minecraft.Components.Text.Colors;
using Void.Minecraft.Nbt.Serializers.Json;
using Void.Minecraft.Nbt.Serializers.String;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Minecraft.Components.Text.Transformers;

public static class ComponentJsonTransformers
{
    private const string ShowAchievementMarker = "!1.11.1=>1.12!";

    public static void Apply(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion from, ProtocolVersion to)
    {
        wrapper.Write(Apply(wrapper.Read<StringProperty>(), from, to));
    }

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
        if (from > ProtocolVersion.MINECRAFT_1_15_2 && to <= ProtocolVersion.MINECRAFT_1_15_2)
            node = Downgrade_v1_16_to_v1_15_2(node);

        if (from > ProtocolVersion.MINECRAFT_1_11_1 && to <= ProtocolVersion.MINECRAFT_1_11_1)
            node = Downgrade_v1_12_to_v1_11_1(node);

        if (from > ProtocolVersion.MINECRAFT_1_8 && to <= ProtocolVersion.MINECRAFT_1_8)
            node = Downgrade_v1_9_to_v1_8(node);

        return node;
    }

    private static JsonNode Upgrade(JsonNode node, ProtocolVersion from, ProtocolVersion to)
    {
        if (from <= ProtocolVersion.MINECRAFT_1_8 && to > ProtocolVersion.MINECRAFT_1_8)
            node = Upgrade_v1_8_to_v1_9(node);

        if (from <= ProtocolVersion.MINECRAFT_1_11_1 && to > ProtocolVersion.MINECRAFT_1_11_1)
            node = Upgrade_v1_11_1_to_v1_12(node);

        if (from <= ProtocolVersion.MINECRAFT_1_15_2 && to > ProtocolVersion.MINECRAFT_1_15_2)
            node = Upgrade_v1_15_2_to_v1_16(node);

        return node;
    }

    #region Downgrade
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

    public static void Passthrough_v1_12_to_v1_11_1(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Downgrade_v1_12_to_v1_11_1(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }

    public static void Passthrough_v1_9_to_v1_8(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Downgrade_v1_9_to_v1_8(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }
    #endregion

    #region Upgrade
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

    public static void Passthrough_v1_11_1_to_v1_12(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Upgrade_v1_11_1_to_v1_12(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }

    public static void Passthrough_v1_8_to_v1_9(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<StringProperty>();

        if (TryParse(property.AsPrimitive, out var node))
        {
            node = Upgrade_v1_8_to_v1_9(node);
            property = StringProperty.FromPrimitive(node.ToString());
        }

        wrapper.Write(property);
    }
    #endregion

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
                rootObject["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_12_to_v1_11_1(childNode)))]);

            if (rootObject["extra"] is JsonArray extra)
                rootObject["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_12_to_v1_11_1(childNode)))]);
        }

        return node;
    }

    public static JsonNode Downgrade_v1_9_to_v1_8(JsonNode node)
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
                        if (value.GetValueKind() is JsonValueKind.Object)
                        {
                            if (action.GetValue<string>() is "show_achievement" or "show_entity" or "show_item")
                            {
                                if (value["text"] is not { } text)
                                    throw new NotSupportedException($"Text value not found: {value}");

                                hoverEvent["value"] = text;
                            }
                        }
                    }
                }
            }

            // Replace recursive text components
            if (rootObject["with"] is JsonArray with)
                rootObject["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_9_to_v1_8(childNode)))]);

            if (rootObject["extra"] is JsonArray extra)
                rootObject["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Downgrade_v1_9_to_v1_8(childNode)))]);
        }

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

        // De-compact text component
        if (node.GetValueKind() is JsonValueKind.String)
            node = new JsonObject { ["text"] = JsonSerializer.SerializeToNode(node) };

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

    public static JsonNode Upgrade_v1_8_to_v1_9(JsonNode node)
    {
        if (node is JsonObject root)
        {
            if (root["hoverEvent"] is JsonObject hoverEvent)
            {
                if (hoverEvent["value"] is { } value)
                {
                    if (hoverEvent["action"] is { } action)
                    {
                        if (action.GetValue<string>() is "show_achievement" or "show_entity" or "show_item")
                        {
                            if (value is JsonObject valueAsObject && valueAsObject.ContainsKey("text"))
                                hoverEvent["value"] = value; // Minecraft Console Clients sends JsonObject as value
                            else
                                hoverEvent["value"] = new JsonObject { ["text"] = JsonSerializer.SerializeToNode(value) }; // Vanilla behavior
                        }
                    }
                }
            }

            // Replace recursive text components
            if (root["with"] is JsonArray with)
                root["with"] = new JsonArray([.. with.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_8_to_v1_9(childNode)))]);

            if (root["extra"] is JsonArray extra)
                root["extra"] = new JsonArray([.. extra.WhereNotNull().Select(childNode => JsonSerializer.SerializeToNode(Upgrade_v1_8_to_v1_9(childNode)))]);
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
