using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Components.Text.Colors;
using Void.Minecraft.Components.Text.Events;
using Void.Minecraft.Components.Text.Events.Actions;
using Void.Minecraft.Components.Text.Events.Actions.Click;
using Void.Minecraft.Components.Text.Events.Actions.Hover;
using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Serializers;

public static class JsonComponentSerializer
{
    // TODO WARNING this class is fully ChatGPT-generated, please rewrite it ASAP

    public static JsonObject Serialize(Component component, ProtocolVersion protocolVersion)
    {
        var content = component.Content;
        var json = new JsonObject
        {
            ["type"] = content.Type
        };

        switch (content)
        {
            case TextContent textContent:
                json["text"] = textContent.Value;
                break;
            case TranslatableContent translatableContent:
                json["translate"] = translatableContent.Translate;

                if (translatableContent.Fallback is not null)
                    json["fallback"] = translatableContent.Fallback;

                if (translatableContent.With is not null)
                    json["with"] = new JsonArray(translatableContent.With.Select(child => Serialize(child, protocolVersion)).ToArray());
                break;
            case ScoreContent scoreContent:
                json["score"] = new JsonObject
                {
                    ["name"] = scoreContent.Name,
                    ["objective"] = scoreContent.Objective
                };
                break;
            case SelectorContent selectorContent:
                json["selector"] = selectorContent.Value;

                if (selectorContent.Separator is not null)
                    json["separator"] = Serialize(selectorContent.Separator, protocolVersion);
                break;
            case KeybindContent keybindContent:
                json["keybind"] = keybindContent.Value;
                break;
            case NbtContent nbtContent:
                // No implementation, as in the original code.
                break;
            default:
                throw new NotSupportedException($"Component {content} serialization is not supported");
        }

        var extra = component.Children.Extra;

        if (extra.Any())
            json["extra"] = new JsonArray(extra.Select(child => Serialize(child, protocolVersion)).ToArray());

        var formatting = component.Formatting;

        if (formatting != Formatting.Default)
        {
            if (formatting.Color is not null)
                json["color"] = formatting.Color.Name;

            if (!string.IsNullOrWhiteSpace(formatting.Font))
                json["font"] = formatting.Font;

            if (formatting.IsBold.HasValue)
                json["bold"] = formatting.IsBold.Value;

            if (formatting.IsItalic.HasValue)
                json["italic"] = formatting.IsItalic.Value;

            if (formatting.IsUnderlined.HasValue)
                json["underlined"] = formatting.IsUnderlined.Value;

            if (formatting.IsStrikethrough.HasValue)
                json["strikethrough"] = formatting.IsStrikethrough.Value;

            if (formatting.IsObfuscated.HasValue)
                json["obfuscated"] = formatting.IsObfuscated.Value;

            if (formatting.ShadowColor is not null)
                json["shadow_color"] = new JsonArray(((float[])formatting.ShadowColor).Select(value => (JsonNode)value).ToArray());
        }

        var interactivity = component.Interactivity;

        if (interactivity != Interactivity.Default)
        {
            if (!string.IsNullOrWhiteSpace(interactivity.Insertion))
                json["insertion"] = interactivity.Insertion;

            var clickEvent = interactivity.ClickEvent;

            if (clickEvent is not null)
            {
                json["clickEvent"] = new JsonObject
                {
                    ["action"] = clickEvent.ActionName,
                    ["value"] = clickEvent.Value
                };
            }

            var hoverEvent = interactivity.HoverEvent;

            if (hoverEvent is not null)
            {
                var action = hoverEvent.ActionName;
                var contents = new JsonObject();

                if (hoverEvent.Content is ShowText showText)
                {
                    contents = Serialize(showText.Value, protocolVersion);
                }

                if (hoverEvent.Content is ShowItem showItem)
                {
                    contents["id"] = showItem.Id;

                    if (showItem.Count.HasValue)
                        contents["count"] = showItem.Count.Value;

                    if (showItem.ItemComponents is not null)
                        contents["components"] = new JsonObject(); // TODO implement item components
                }

                if (hoverEvent.Content is ShowEntity showEntity)
                {
                    contents["id"] = showEntity.Id.ToString();
                    contents["type"] = showEntity.Type;

                    if (showEntity.Name is not null)
                        contents["name"] = Serialize(showEntity.Name, protocolVersion);
                }

                json["hoverEvent"] = new JsonObject
                {
                    ["action"] = action,
                    ["contents"] = contents
                };
            }
        }

        return json;
    }

    public static Component Deserialize(JsonNode node, ProtocolVersion protocolVersion)
    {
        var component = Component.Default;

        if (node is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var text))
            return component with { Content = new TextContent(text) };

        if (node is not JsonObject json)
            throw new JsonException($"Json node {node.GetType()} deserialization is not supported");

        if ((json["type"]?.GetValue<string>() == "text") || json.ContainsKey("text"))
        {
            var textValue = Get<string>(json, "text");
            component = component with { Content = new TextContent(textValue) };
        }

        if ((json["type"]?.GetValue<string>() == "translatable") || json.ContainsKey("translate"))
        {
            var translateValue = Get<string>(json, "translate");
            var fallbackValue = TryGet<string>(json, "fallback");
            var withArray = TryGet<JsonArray>(json, "with");
            var withComponents = withArray?.Select(child =>
            {
                if (child is JsonObject || child is JsonValue)
                    return Deserialize(child, protocolVersion);
                return null;
            }).Where(comp => comp is not null).Cast<Component>();

            component = component with { Content = new TranslatableContent(translateValue, fallbackValue, withComponents) };
        }

        if ((json["type"]?.GetValue<string>() == "score") || json.ContainsKey("score"))
        {
            var scoreObj = Get<JsonObject>(json, "score");
            var scoreName = Get<string>(scoreObj, "name");
            var scoreObjective = Get<string>(scoreObj, "objective");

            component = component with { Content = new ScoreContent(scoreName, scoreObjective) };
        }

        if ((json["type"]?.GetValue<string>() == "selector") || json.ContainsKey("selector"))
        {
            var selectorValue = Get<string>(json, "selector");
            var separatorComponent = TryGet<JsonNode>(json, "separator") is JsonNode sep && (sep is JsonObject || sep is JsonValue)
                ? Deserialize(sep, protocolVersion)
                : null;

            component = component with { Content = new SelectorContent(selectorValue, separatorComponent) };
        }

        if ((json["type"]?.GetValue<string>() == "keybind") || json.ContainsKey("keybind"))
        {
            var keybindValue = Get<string>(json, "keybind");
            component = component with { Content = new KeybindContent(keybindValue) };
        }

        if ((json["type"]?.GetValue<string>() == "nbt") || json.ContainsKey("nbt"))
        {
            var source = TryGet<string>(json, "source");
            var path = Get<string>(json, "nbt");
            var interpret = TryGet<bool>(json, "interpret");
            var separatorComponent = TryGet<JsonNode>(json, "separator") is JsonNode sep && (sep is JsonObject || sep is JsonValue)
                ? Deserialize(sep, protocolVersion)
                : null;
            var block = TryGet<string>(json, "block");
            var entity = TryGet<string>(json, "entity");
            var storage = TryGet<string>(json, "storage");

            component = component with { Content = new NbtContent(path, source, interpret, separatorComponent, block, entity, storage) };
        }

        if (TryGet<JsonArray>(json, "extra") is JsonArray extraArray)
        {
            var extraComponents = extraArray.Select(child =>
            {
                if (child is JsonObject || child is JsonValue)
                    return Deserialize(child, protocolVersion);
                return null;
            }).Where(comp => comp is not null).Cast<Component>();

            if (extraComponents is not null)
                component = component with { Children = component.Children with { Extra = extraComponents } };
        }

        if (TryGet<string>(json, "color") is string colorValue)
            component = component with { Formatting = component.Formatting with { Color = TextColor.FromString(colorValue) } };

        if (TryGet<string>(json, "font") is string fontValue)
            component = component with { Formatting = component.Formatting with { Font = fontValue } };

        if (TryGet<bool>(json, "bold") is bool boldValue)
            component = component with { Formatting = component.Formatting with { IsBold = boldValue } };

        if (TryGet<bool>(json, "italic") is bool italicValue)
            component = component with { Formatting = component.Formatting with { IsItalic = italicValue } };

        if (TryGet<bool>(json, "underlined") is bool underlinedValue)
            component = component with { Formatting = component.Formatting with { IsUnderlined = underlinedValue } };

        if (TryGet<bool>(json, "strikethrough") is bool strikethroughValue)
            component = component with { Formatting = component.Formatting with { IsStrikethrough = strikethroughValue } };

        if (TryGet<bool>(json, "obfuscated") is bool obfuscatedValue)
            component = component with { Formatting = component.Formatting with { IsObfuscated = obfuscatedValue } };

        if (json["shadow_color"] != null)
        {
            if (json["shadow_color"] is JsonArray shadowArray)
                component = component with { Formatting = component.Formatting with { ShadowColor = shadowArray.Select(t => t!.GetValue<float>()).ToArray() } };
            else if (json["shadow_color"]?.GetValue<JsonElement>().ValueKind == JsonValueKind.Number)
                component = component with { Formatting = component.Formatting with { ShadowColor = json["shadow_color"]?.GetValue<int>() } };
        }

        if (TryGet<string>(json, "insertion") is string insertionValue)
            component = component with { Interactivity = component.Interactivity with { Insertion = insertionValue } };

        if (TryGet<JsonObject>(json, "clickEvent") is JsonObject clickEventObj)
        {
            var actionValue = Get<string>(clickEventObj, "action");
            var valueValue = Get<string>(clickEventObj, "value");

            var action = actionValue switch
            {
                "open_url" => new OpenUrl() as IClickEventAction,
                "open_file" => new OpenFile(),
                "run_command" => new RunCommand(),
                "suggest_command" => new SuggestCommand(),
                "change_page" => new ChangePage(),
                "copy_to_clipboard" => new CopyToClipboard(),
                var a => throw new NotSupportedException(a)
            };

            component = component with { Interactivity = component.Interactivity with { ClickEvent = new ClickEvent(action, valueValue) } };
        }

        if (TryGet<JsonObject>(json, "hoverEvent") is JsonObject hoverEventObj)
        {
            var actionValue = Get<string>(hoverEventObj, "action");
            var contentsNode = Get<JsonNode>(hoverEventObj, "contents");

            var contentAction = actionValue switch
            {
                "show_text" => (IHoverEventAction)(contentsNode is JsonObject || contentsNode is JsonValue
                    ? new ShowText(Deserialize(contentsNode, protocolVersion))
                    : throw new JsonException(contentsNode.ToString())),
                "show_item" => contentsNode is JsonObject contentsObj
                    ? new ShowItem(Get<string>(contentsObj, "id"), TryGet<int>(contentsObj, "type"), null)
                    : throw new JsonException(contentsNode.ToString()),
                "show_entity" => contentsNode is JsonObject contentsObj2
                    ? new ShowEntity(Get<string>(contentsObj2, "type"),
                        Get<JsonNode>(contentsObj2, "id") is JsonValue idValue && idValue.TryGetValue<string>(out var idStr)
                            ? Uuid.Parse(idStr)
                            : Uuid.Empty)
                    : throw new JsonException(contentsNode.ToString()),
                var a => throw new NotSupportedException(a)
            };

            component = component with { Interactivity = component.Interactivity with { HoverEvent = new HoverEvent(contentAction) } };
        }

        return component;

        static T Get<T>(JsonObject json, string key)
        {
            return TryGet<T>(json, key) ?? throw new JsonException(json[key]?.ToString() ?? key);
        }

        static T? TryGet<T>(JsonObject json, string key)
        {
            if (json.TryGetPropertyValue(key, out JsonNode? node))
            {
                if (typeof(T) == typeof(string))
                    return (T?)(object?)node?.GetValue<string>();
                if (typeof(T) == typeof(bool))
                    return (T?)(object?)node?.GetValue<bool>();
                if (typeof(T) == typeof(int))
                    return (T?)(object?)node?.GetValue<int>();
                if (typeof(T) == typeof(float))
                    return (T?)(object?)node?.GetValue<float>();
                if (typeof(T) == typeof(JsonObject))
                    return (T?)(object?)node?.AsObject();
                if (typeof(T) == typeof(JsonArray))
                    return (T?)(object?)node?.AsArray();
            }
            return default;
        }
    }
}
