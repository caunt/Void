using System;
using System.Linq;
using Void.Minecraft.Components.Text.Colors;
using Void.Minecraft.Components.Text.Events;
using Void.Minecraft.Components.Text.Events.Actions;
using Void.Minecraft.Components.Text.Events.Actions.Click;
using Void.Minecraft.Components.Text.Events.Actions.Hover;
using Void.Minecraft.Components.Text.Exceptions;
using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;
using Void.Minecraft.Nbt;
using Void.Minecraft.Nbt.Tags;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Serializers;

public static class NbtComponentSerializer
{
    public static NbtCompound Serialize(Component component, ProtocolVersion protocolVersion)
    {
        var content = component.Content;
        var tag = new NbtCompound
        {
            ["type"] = new NbtString(content.Type)
        };

        switch (content)
        {
            case TextContent textContent:
                tag["text"] = new NbtString(textContent.Value);
                break;
            case TranslatableContent translatableContent:
                tag["translate"] = new NbtString(translatableContent.Translate);

                if (translatableContent.Fallback is not null)
                    tag["fallback"] = new NbtString(translatableContent.Fallback);

                if (translatableContent.With is not null)
                    tag["with"] = new NbtList(translatableContent.With.Select(component => Serialize(component, protocolVersion)), NbtTagType.Compound);
                break;
            case ScoreContent scoreContent:
                tag["score"] = new NbtCompound
                {
                    ["name"] = new NbtString(scoreContent.Name),
                    ["objective"] = new NbtString(scoreContent.Objective)
                };
                break;
            case SelectorContent selectorContent:
                tag["selector"] = new NbtString(selectorContent.Value);

                if (selectorContent.Separator is not null)
                    tag["separator"] = Serialize(selectorContent.Separator, protocolVersion);
                break;
            case KeybindContent keybindContent:
                tag["keybind"] = new NbtString(keybindContent.Value);
                break;
            case NbtContent nbtContent:
                break;
            default:
                throw new NotSupportedException($"Component {content} serialization is not supported");
        }

        var extra = component.Children.Extra;

        if (extra.Any())
            tag["extra"] = new NbtList(extra.Select(component => Serialize(component, protocolVersion)), NbtTagType.Compound);

        var formatting = component.Formatting;

        if (formatting != Formatting.Default)
        {
            if (formatting.Color is not null)
                tag["color"] = new NbtString(formatting.Color.Name);

            if (!string.IsNullOrWhiteSpace(formatting.Font))
                tag["font"] = new NbtString(formatting.Font);

            if (formatting.IsBold.HasValue)
                tag["bold"] = new NbtBoolean(formatting.IsBold.Value);

            if (formatting.IsItalic.HasValue)
                tag["italic"] = new NbtBoolean(formatting.IsItalic.Value);

            if (formatting.IsUnderlined.HasValue)
                tag["underlined"] = new NbtBoolean(formatting.IsUnderlined.Value);

            if (formatting.IsStrikethrough.HasValue)
                tag["strikethrough"] = new NbtBoolean(formatting.IsStrikethrough.Value);

            if (formatting.IsObfuscated.HasValue)
                tag["obfuscated"] = new NbtBoolean(formatting.IsObfuscated.Value);

            if (formatting.ShadowColor is not null)
                tag["shadow_color"] = new NbtList(((float[])formatting.ShadowColor).Select(value => new NbtFloat(value)), NbtTagType.Float);
        }

        var interactivity = component.Interactivity;

        if (interactivity != Interactivity.Default)
        {
            if (!string.IsNullOrWhiteSpace(interactivity.Insertion))
                tag["insertion"] = new NbtString(interactivity.Insertion);

            var clickEvent = interactivity.ClickEvent;

            if (clickEvent is not null)
            {
                tag["clickEvent"] = new NbtCompound
                {
                    ["action"] = new NbtString(clickEvent.ActionName),
                    ["value"] = new NbtString(clickEvent.Value)
                };
            }

            var hoverEvent = interactivity.HoverEvent;

            if (hoverEvent is not null)
            {
                var action = new NbtString(hoverEvent.ActionName);
                var contents = new NbtCompound();

                if (hoverEvent.Content is ShowText { } showText)
                {
                    contents = Serialize(showText.Value, protocolVersion);
                }

                if (hoverEvent.Content is ShowItem { } showItem)
                {
                    contents["id"] = new NbtString(showItem.Id);

                    if (showItem.Count.HasValue)
                        contents["count"] = new NbtInt(showItem.Count.Value);

                    if (showItem.ItemComponents is not null)
                        contents["components"] = new NbtCompound(); // TODO implement item components
                }

                if (hoverEvent.Content is ShowEntity { } showEntity)
                {
                    contents["id"] = new NbtString(showEntity.Id.ToString());
                    contents["type"] = new NbtString(showEntity.Type);

                    if (showEntity.Name is not null)
                        contents["name"] = Serialize(showEntity.Name, protocolVersion);
                }

                tag["hoverEvent"] = new NbtCompound
                {
                    ["action"] = action,
                    ["contents"] = contents
                };
            }
        }

        return tag;
    }

    public static Component Deserialize(NbtTag tag, ProtocolVersion protocolVersion)
    {
        var component = Component.Default;

        if (tag is NbtString tagNbtString)
            return component with { Content = new TextContent(tagNbtString.Value) };

        if (tag is not NbtCompound tagNbtCompound)
            throw new NbtException($"Nbt tag {tag.Type} deserialization is not supported");

        if (tagNbtCompound["type"] is NbtString { Value: "text" } || tagNbtCompound.ContainsKey("text"))
        {
            var textNbtString = Get<NbtString>(tagNbtCompound, "text");

            component = component with { Content = new TextContent(textNbtString.Value) };
        }

        if (tagNbtCompound["type"] is NbtString { Value: "translatable" } || tagNbtCompound.ContainsKey("translate"))
        {
            var translateNbtString = Get<NbtString>(tagNbtCompound, "translate");
            var fallbackNbtString = TryGet<NbtString>(tagNbtCompound, "fallback");
            var withNbtList = TryGet<NbtList>(tagNbtCompound, "with") switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => value.Data.Select(nbt => Deserialize(nbt, protocolVersion)),
                _ => null
            };

            component = component with { Content = new TranslatableContent(translateNbtString.Value, fallbackNbtString?.Value, withNbtList) };
        }

        if (tagNbtCompound["type"] is NbtString { Value: "score" } || tagNbtCompound.ContainsKey("score"))
        {
            var scoreNbtCompound = Get<NbtCompound>(tagNbtCompound, "score");
            var scoreNameNbtString = Get<NbtString>(scoreNbtCompound, "name");
            var scoreObjectiveNbtString = Get<NbtString>(scoreNbtCompound, "objective");

            component = component with { Content = new ScoreContent(scoreNameNbtString.Value, scoreObjectiveNbtString.Value) };
        }

        if (tagNbtCompound["type"] is NbtString { Value: "selector" } || tagNbtCompound.ContainsKey("selector"))
        {
            var selectorNbtString = Get<NbtString>(tagNbtCompound, "selector");
            var separatorComponent = TryGet<NbtTag>(tagNbtCompound, "separator") switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value, protocolVersion),
                _ => null
            };

            component = component with { Content = new SelectorContent(selectorNbtString.Value, separatorComponent) };
        }

        if (tagNbtCompound["type"] is NbtString { Value: "keybind" } || tagNbtCompound.ContainsKey("keybind"))
        {
            var keybindNbtString = Get<NbtString>(tagNbtCompound, "keybind");

            component = component with { Content = new KeybindContent(keybindNbtString.Value) };
        }

        if (tagNbtCompound["type"] is NbtString { Value: "nbt" } || tagNbtCompound.ContainsKey("nbt"))
        {
            var sourceNbtString = TryGet<NbtString>(tagNbtCompound, "source");
            var pathNbtString = Get<NbtString>(tagNbtCompound, "nbt");
            var interpretNbtString = TryGet<NbtBoolean>(tagNbtCompound, "interpret");
            var separatorComponent = TryGet<NbtTag>(tagNbtCompound, "separator") switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value, protocolVersion),
                _ => null
            };
            var blockNbtString = TryGet<NbtString>(tagNbtCompound, "block");
            var entityNbtString = TryGet<NbtString>(tagNbtCompound, "entity");
            var storageNbtString = TryGet<NbtString>(tagNbtCompound, "storage");

            component = component with { Content = new NbtContent(pathNbtString.Value, sourceNbtString?.Value, interpretNbtString?.Value, separatorComponent, blockNbtString?.Value, entityNbtString?.Value, storageNbtString?.Value) };
        }

        if (TryGet<NbtList>(tagNbtCompound, "extra") is { } extraNbtList)
        {
            var extraComponents = extraNbtList.Data.Select(dataTag => dataTag switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value, protocolVersion),
                _ => null
            }).Where(component => component is not null).Cast<Component>();

            if (extraComponents is not null)
                component = component with { Children = component.Children with { Extra = extraComponents } };
        }

        if (TryGet<NbtString>(tagNbtCompound, "color") is { } colorNbtString)
            component = component with { Formatting = component.Formatting with { Color = TextColor.FromString(colorNbtString.Value) } };

        if (TryGet<NbtString>(tagNbtCompound, "font") is { } fontNbtString)
            component = component with { Formatting = component.Formatting with { Font = fontNbtString.Value } };

        if (TryGet<NbtBoolean>(tagNbtCompound, "bold") is { } boldNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsBold = boldNbtBoolean.Value } };

        if (TryGet<NbtBoolean>(tagNbtCompound, "italic") is { } italicNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsItalic = italicNbtBoolean.Value } };

        if (TryGet<NbtBoolean>(tagNbtCompound, "underlined") is { } underlinedNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsUnderlined = underlinedNbtBoolean.Value } };

        if (TryGet<NbtBoolean>(tagNbtCompound, "strikethrough") is { } strikethroughNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsStrikethrough = strikethroughNbtBoolean.Value } };

        if (TryGet<NbtBoolean>(tagNbtCompound, "obfuscated") is { } obfuscatedNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsObfuscated = obfuscatedNbtBoolean.Value } };

        if (TryGet<NbtTag>(tagNbtCompound, "shadow_color") is { } shadowColorNbtTag)
        {
            if (shadowColorNbtTag is NbtList { DataType: NbtTagType.Float } shadowColorNbtList)
                component = component with { Formatting = component.Formatting with { ShadowColor = shadowColorNbtList.Data.Select(dataTag => ((NbtFloat)dataTag).Value).ToArray() } };
            else if (shadowColorNbtTag is NbtInt shadowColorNbtInt)
                component = component with { Formatting = component.Formatting with { ShadowColor = shadowColorNbtInt.Value } };
        }

        if (TryGet<NbtString>(tagNbtCompound, "insertion") is { } insertionNbtString)
            component = component with { Interactivity = component.Interactivity with { Insertion = insertionNbtString.Value } };

        if (TryGet<NbtCompound>(tagNbtCompound, "clickEvent") is { } clickEventNbtCompound)
        {
            var actionNbtString = Get<NbtString>(clickEventNbtCompound, "action");
            var valueNbtString = Get<NbtString>(clickEventNbtCompound, "value");

            var action = actionNbtString.Value switch
            {
                "open_url" => new OpenUrl() as IClickEventAction,
                "open_file" => new OpenFile(),
                "run_command" => new RunCommand(),
                "suggest_command" => new SuggestCommand(),
                "change_page" => new ChangePage(),
                "copy_to_clipboard" => new CopyToClipboard(),
                var value => throw new NotSupportedException(value)
            };

            component = component with { Interactivity = component.Interactivity with { ClickEvent = new ClickEvent(action, valueNbtString.Value) } };
        }

        if (TryGet<NbtCompound>(tagNbtCompound, "hoverEvent") is { } hoverEventNbtCompound)
        {
            var actionNbtString = Get<NbtString>(hoverEventNbtCompound, "action");
            var contentsNbtTag = Get<NbtTag>(hoverEventNbtCompound, "contents");

            var content = actionNbtString.Value switch
            {
                "show_text" => contentsNbtTag switch
                {
                    NbtString or NbtCompound => new ShowText(Deserialize(contentsNbtTag, protocolVersion)),
                    var value => throw new NbtException(value)
                } as IHoverEventAction,
                "show_item" => contentsNbtTag switch
                {
                    NbtCompound contentsNbtCompoundTag => new ShowItem(Get<NbtString>(contentsNbtCompoundTag, "id").Value, TryGet<NbtInt>(contentsNbtCompoundTag, "type")?.Value, null),
                    var value => throw new NbtException(value)
                },
                "show_entity" => contentsNbtTag switch
                {
                    NbtCompound contentsNbtCompoundTag => new ShowEntity(Get<NbtString>(contentsNbtCompoundTag, "type").Value, Get<NbtTag>(contentsNbtCompoundTag, "id") switch
                    {
                        NbtString idNbtString => Uuid.Parse(idNbtString.Value),
                        NbtList { DataType: NbtTagType.Int } idNbtList => Uuid.Parse([.. idNbtList.Data.Select(dataTag => ((NbtInt)dataTag).Value)]),
                        var value => throw new NbtException(value)
                    }),
                    var value => throw new NbtException(value)
                },
                var value => throw new NotSupportedException(value)
            };

            component = component with { Interactivity = component.Interactivity with { HoverEvent = new HoverEvent(content) } };
        }

        return component;

        static T Get<T>(NbtCompound tag, string key) where T : NbtTag
        {
            return TryGet<T>(tag, key) ?? throw new NbtException(tag[key]?.Type.ToString() ?? key);
        }

        static T? TryGet<T>(NbtCompound tag, string key) where T : NbtTag
        {
            return tag[key] as T;
        }
    }
}
