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

public static class ComponentNbtSerializer
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
                tag["bold"] = new NbtByte(formatting.IsBold.Value);

            if (formatting.IsItalic.HasValue)
                tag["italic"] = new NbtByte(formatting.IsItalic.Value);

            if (formatting.IsUnderlined.HasValue)
                tag["underlined"] = new NbtByte(formatting.IsUnderlined.Value);

            if (formatting.IsStrikethrough.HasValue)
                tag["strikethrough"] = new NbtByte(formatting.IsStrikethrough.Value);

            if (formatting.IsObfuscated.HasValue)
                tag["obfuscated"] = new NbtByte(formatting.IsObfuscated.Value);

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
                        contents["components"] = showItem.ItemComponents;
                }

                if (hoverEvent.Content is ShowEntity { } showEntity)
                {
                    contents["id"] = new NbtString(showEntity.Id.ToString());

                    if (showEntity.Type is not null)
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

        if (tag is NbtString nbtString)
            return component with { Content = new TextContent(nbtString.Value) };

        DeserializeContent(ref component, tag, protocolVersion);
        DeserializeChildren(ref component, tag, protocolVersion);
        DeserializeFormatting(ref component, tag, protocolVersion);
        DeserializeInteractivity(ref component, tag, protocolVersion);

        return component;
    }

    private static NbtCompound AsCompound(this NbtTag tag)
    {
        if (tag is not NbtCompound nbtCompound)
            throw new NbtException($"Nbt tag {tag.Type} deserialization is not supported");

        return nbtCompound;
    }

    private static T Get<T>(this NbtCompound tag, string key) where T : NbtTag
    {
        return TryGet<T>(tag, key) ?? throw new NbtException($"Tag \"{tag[key]?.Type.ToString() ?? key}\" not found");
    }

    private static T? TryGet<T>(this NbtCompound tag, string key) where T : NbtTag
    {
        return tag[key] as T;
    }

    private static void DeserializeContent(ref Component component, NbtTag tag, ProtocolVersion protocolVersion)
    {
        var compound = tag.AsCompound();

        if (compound["type"] is NbtString { Value: "text" } || compound.ContainsKey("text"))
        {
            var textNbtString = Get<NbtString>(compound, "text");

            component = component with { Content = new TextContent(textNbtString.Value) };
        }

        if (compound["type"] is NbtString { Value: "translatable" } || compound.ContainsKey("translate"))
        {
            var translateNbtString = Get<NbtString>(compound, "translate");
            var fallbackNbtString = TryGet<NbtString>(compound, "fallback");
            var withNbtList = TryGet<NbtList>(compound, "with");
            var withComponents = withNbtList?.Data.Select(dataTag => dataTag switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value, protocolVersion),
                _ => null
            }).Where(component => component is not null).Cast<Component>();

            component = component with { Content = new TranslatableContent(translateNbtString.Value, fallbackNbtString?.Value, withComponents) };
        }

        if (compound["type"] is NbtString { Value: "score" } || compound.ContainsKey("score"))
        {
            var scoreNbtCompound = Get<NbtCompound>(compound, "score");
            var scoreNameNbtString = Get<NbtString>(scoreNbtCompound, "name");
            var scoreObjectiveNbtString = Get<NbtString>(scoreNbtCompound, "objective");

            component = component with { Content = new ScoreContent(scoreNameNbtString.Value, scoreObjectiveNbtString.Value) };
        }

        if (compound["type"] is NbtString { Value: "selector" } || compound.ContainsKey("selector"))
        {
            var selectorNbtString = Get<NbtString>(compound, "selector");
            var separatorComponent = TryGet<NbtTag>(compound, "separator") switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value, protocolVersion),
                _ => null
            };

            component = component with { Content = new SelectorContent(selectorNbtString.Value, separatorComponent) };
        }

        if (compound["type"] is NbtString { Value: "keybind" } || compound.ContainsKey("keybind"))
        {
            var keybindNbtString = Get<NbtString>(compound, "keybind");

            component = component with { Content = new KeybindContent(keybindNbtString.Value) };
        }

        if (compound["type"] is NbtString { Value: "nbt" } || compound.ContainsKey("nbt"))
        {
            var sourceNbtString = TryGet<NbtString>(compound, "source");
            var pathNbtString = Get<NbtString>(compound, "nbt");
            var interpretNbtString = TryGet<NbtByte>(compound, "interpret");
            var separatorComponent = TryGet<NbtTag>(compound, "separator") switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value, protocolVersion),
                _ => null
            };
            var blockNbtString = TryGet<NbtString>(compound, "block");
            var entityNbtString = TryGet<NbtString>(compound, "entity");
            var storageNbtString = TryGet<NbtString>(compound, "storage");

            component = component with { Content = new NbtContent(pathNbtString.Value, sourceNbtString?.Value, interpretNbtString?.IsTrue, separatorComponent, blockNbtString?.Value, entityNbtString?.Value, storageNbtString?.Value) };
        }
    }

    private static void DeserializeChildren(ref Component component, NbtTag tag, ProtocolVersion protocolVersion)
    {
        var compound = tag.AsCompound();

        if (TryGet<NbtList>(compound, "extra") is { } extraNbtList)
        {
            var extraComponents = extraNbtList.Data.Select(dataTag => dataTag switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value, protocolVersion),
                _ => null
            }).Where(component => component is not null).Cast<Component>();

            if (extraComponents is not null)
                component = component with { Children = component.Children with { Extra = extraComponents } };
        }
    }

    private static void DeserializeFormatting(ref Component component, NbtTag tag, ProtocolVersion protocolVersion)
    {
        var compound = tag.AsCompound();

        if (TryGet<NbtString>(compound, "color") is { } colorNbtString)
            component = component with { Formatting = component.Formatting with { Color = TextColor.FromString(colorNbtString.Value) } };

        if (TryGet<NbtString>(compound, "font") is { } fontNbtString)
            component = component with { Formatting = component.Formatting with { Font = fontNbtString.Value } };

        if (TryGet<NbtByte>(compound, "bold") is { } boldNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsBold = boldNbtBoolean.IsTrue } };

        if (TryGet<NbtByte>(compound, "italic") is { } italicNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsItalic = italicNbtBoolean.IsTrue } };

        if (TryGet<NbtByte>(compound, "underlined") is { } underlinedNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsUnderlined = underlinedNbtBoolean.IsTrue } };

        if (TryGet<NbtByte>(compound, "strikethrough") is { } strikethroughNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsStrikethrough = strikethroughNbtBoolean.IsTrue } };

        if (TryGet<NbtByte>(compound, "obfuscated") is { } obfuscatedNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsObfuscated = obfuscatedNbtBoolean.IsTrue } };

        if (TryGet<NbtTag>(compound, "shadow_color") is { } shadowColorNbtTag)
        {
            if (shadowColorNbtTag is NbtList { DataType: NbtTagType.Float } shadowColorNbtList)
                component = component with { Formatting = component.Formatting with { ShadowColor = shadowColorNbtList.Data.Select(dataTag => ((NbtFloat)dataTag).Value).ToArray() } };
            else if (shadowColorNbtTag is NbtInt shadowColorNbtInt)
                component = component with { Formatting = component.Formatting with { ShadowColor = shadowColorNbtInt.Value } };
        }
    }

    private static void DeserializeInteractivity(ref Component component, NbtTag tag, ProtocolVersion protocolVersion)
    {
        var compound = tag.AsCompound();

        if (TryGet<NbtString>(compound, "insertion") is { } insertionNbtString)
            component = component with { Interactivity = component.Interactivity with { Insertion = insertionNbtString.Value } };

        if (TryGet<NbtCompound>(compound, "clickEvent") is { } clickEventNbtCompound)
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

        if (TryGet<NbtCompound>(compound, "hoverEvent") is { } hoverEventNbtCompound)
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
                    NbtCompound contentsNbtCompoundTag => new ShowItem(Get<NbtString>(contentsNbtCompoundTag, "id").Value, TryGet<NbtInt>(contentsNbtCompoundTag, "type")?.Value, Get<NbtCompound>(contentsNbtCompoundTag, "components")),
                    var value => throw new NbtException(value)
                },
                "show_entity" => contentsNbtTag switch
                {
                    NbtCompound contentsNbtCompoundTag => new ShowEntity(Get<NbtTag>(contentsNbtCompoundTag, "id") switch
                    {
                        NbtString idNbtString => Uuid.Parse(idNbtString.Value),
                        NbtIntArray idNbtIntArray => Uuid.Parse([.. idNbtIntArray.Data]),
                        var value => throw new NbtException(value)
                    },
                    TryGet<NbtString>(contentsNbtCompoundTag, "type")?.Value,
                    TryGet<NbtTag>(contentsNbtCompoundTag, "name") switch
                    {
                        { } nameTag => Deserialize(nameTag, protocolVersion),
                        _ => null
                    }),
                    var value => throw new NbtException(value)
                },
                var value => throw new NotSupportedException(value)
            };

            component = component with { Interactivity = component.Interactivity with { HoverEvent = new HoverEvent(content) } };
        }
    }
}
