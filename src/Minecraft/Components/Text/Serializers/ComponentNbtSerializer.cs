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
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Serializers;

public static class ComponentNbtSerializer
{
    public static NbtCompound Serialize(Component component)
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
                    tag["with"] = new NbtList(translatableContent.With.Select(component => Serialize(component)), NbtTagType.Compound);
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
                    tag["separator"] = Serialize(selectorContent.Separator);
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
            tag["extra"] = new NbtList(extra.Select(component => Serialize(component)), NbtTagType.Compound);

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
                var clickEventTag = new NbtCompound
                {
                    ["action"] = new NbtString(clickEvent.ActionName),
                };

                switch (clickEvent.Content)
                {
                    case OpenUrl openUrl:
                        clickEventTag["url"] = new NbtString(openUrl.Url);
                        break;
                    case OpenFile openFile:
                        clickEventTag["value"] = new NbtString(openFile.File);
                        break;
                    case RunCommand runCommand:
                        clickEventTag["command"] = new NbtString(runCommand.Command);
                        break;
                    case SuggestCommand suggestCommand:
                        clickEventTag["command"] = new NbtString(suggestCommand.Command);
                        break;
                    case ChangePage changePage:
                        clickEventTag["page"] = new NbtInt(changePage.Page);
                        break;
                    case CopyToClipboard copyToClipboard:
                        clickEventTag["value"] = new NbtString(copyToClipboard.Value);
                        break;
                }

                tag["click_event"] = clickEventTag;
            }

            var hoverEvent = interactivity.HoverEvent;

            if (hoverEvent is not null)
            {
                var action = new NbtString(hoverEvent.ActionName);
                var hoverEventTag = new NbtCompound
                {
                    ["action"] = action
                };

                if (hoverEvent.Content is ShowText { } showText)
                {
                    hoverEventTag["value"] = Serialize(showText.Value);
                }

                if (hoverEvent.Content is ShowItem { } showItem)
                {
                    hoverEventTag["id"] = new NbtString(showItem.Id);

                    if (showItem.Count.HasValue)
                        hoverEventTag["count"] = new NbtInt(showItem.Count.Value);

                    if (showItem.ItemComponents is not null)
                        hoverEventTag["components"] = showItem.ItemComponents;
                }

                if (hoverEvent.Content is ShowEntity { } showEntity)
                {
                    hoverEventTag["uuid"] = new NbtString(showEntity.Id.ToString());

                    if (showEntity.Type is not null)
                        hoverEventTag["id"] = new NbtString(showEntity.Type);

                    if (showEntity.Name is not null)
                        hoverEventTag["name"] = Serialize(showEntity.Name);
                }

                tag["hover_event"] = hoverEventTag;
            }
        }

        return tag;
    }

    public static Component Deserialize(NbtTag tag)
    {
        var component = Component.Default;

        if (tag is NbtEnd)
            return component;

        if (tag is NbtString nbtString)
            return component with { Content = new TextContent(nbtString.Value) };

        DeserializeContent(ref component, tag);
        DeserializeChildren(ref component, tag);
        DeserializeFormatting(ref component, tag);
        DeserializeInteractivity(ref component, tag);

        return component;
    }

    private static NbtCompound AsCompound(this NbtTag tag)
    {
        if (tag is not NbtCompound nbtCompound)
            throw new NbtException($"Nbt tag {tag.Type} deserialization is not supported");

        return nbtCompound;
    }

    private static T GetAs<T>(this NbtCompound tag, string key) where T : NbtTag
    {
        if (TryGetAs<T>(tag, key) is { } value)
            return value;

        if (tag.TryGetValue(key, out var invalidTag))
            throw new NbtException($"Tag \"{key}\" is a {invalidTag.GetType().Name} type (expected {typeof(T).Name})");

        throw new NbtException($"Tag \"{key}\" not found");
    }

    private static T? TryGetAs<T>(this NbtCompound tag, string key) where T : NbtTag
    {
        return tag[key] as T;
    }

    private static void DeserializeContent(ref Component component, NbtTag tag)
    {
        var compound = tag.AsCompound();

        if (compound["type"] is NbtString { Value: "text" } || compound.ContainsKey("text"))
        {
            // TODO: Number, Boolean and String (nbt)
            var textNbtString = GetAs<NbtString>(compound, "text");

            component = component with { Content = new TextContent(textNbtString.Value) };
        }

        if (compound["type"] is NbtString { Value: "translatable" } || compound.ContainsKey("translate"))
        {
            var translateNbtString = GetAs<NbtString>(compound, "translate");
            var fallbackNbtString = TryGetAs<NbtString>(compound, "fallback");
            var withNbtList = TryGetAs<NbtList>(compound, "with");
            var withComponents = withNbtList?.Data.Select(dataTag => dataTag switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value),
                _ => null
            }).Where(component => component is not null).Cast<Component>();

            component = component with { Content = new TranslatableContent(translateNbtString.Value, fallbackNbtString?.Value, withComponents) };
        }

        if (compound["type"] is NbtString { Value: "score" } || compound.ContainsKey("score"))
        {
            var scoreNbtCompound = GetAs<NbtCompound>(compound, "score");
            var scoreNameNbtString = GetAs<NbtString>(scoreNbtCompound, "name");
            var scoreObjectiveNbtString = GetAs<NbtString>(scoreNbtCompound, "objective");

            component = component with { Content = new ScoreContent(scoreNameNbtString.Value, scoreObjectiveNbtString.Value) };
        }

        if (compound["type"] is NbtString { Value: "selector" } || compound.ContainsKey("selector"))
        {
            var selectorNbtString = GetAs<NbtString>(compound, "selector");
            var separatorComponent = TryGetAs<NbtTag>(compound, "separator") switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value),
                _ => null
            };

            component = component with { Content = new SelectorContent(selectorNbtString.Value, separatorComponent) };
        }

        if (compound["type"] is NbtString { Value: "keybind" } || compound.ContainsKey("keybind"))
        {
            var keybindNbtString = GetAs<NbtString>(compound, "keybind");

            component = component with { Content = new KeybindContent(keybindNbtString.Value) };
        }

        if (compound["type"] is NbtString { Value: "nbt" } || compound.ContainsKey("nbt"))
        {
            var sourceNbtString = TryGetAs<NbtString>(compound, "source");
            var pathNbtString = GetAs<NbtString>(compound, "nbt");
            var interpretNbtString = TryGetAs<NbtByte>(compound, "interpret");
            var separatorComponent = TryGetAs<NbtTag>(compound, "separator") switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value),
                _ => null
            };
            var blockNbtString = TryGetAs<NbtString>(compound, "block");
            var entityNbtString = TryGetAs<NbtString>(compound, "entity");
            var storageNbtString = TryGetAs<NbtString>(compound, "storage");

            component = component with { Content = new NbtContent(pathNbtString.Value, sourceNbtString?.Value, interpretNbtString?.IsTrue, separatorComponent, blockNbtString?.Value, entityNbtString?.Value, storageNbtString?.Value) };
        }
    }

    private static void DeserializeChildren(ref Component component, NbtTag tag)
    {
        var compound = tag.AsCompound();

        if (TryGetAs<NbtList>(compound, "extra") is { } extraNbtList)
        {
            var extraComponents = extraNbtList.Data.Select(dataTag => dataTag switch
            {
                { Type: NbtTagType.Compound or NbtTagType.String } value => Deserialize(value),
                _ => null
            }).Where(component => component is not null).Cast<Component>();

            if (extraComponents is not null)
                component = component with { Children = component.Children with { Extra = extraComponents } };
        }
    }

    private static void DeserializeFormatting(ref Component component, NbtTag tag)
    {
        var compound = tag.AsCompound();

        if (TryGetAs<NbtString>(compound, "color") is { } colorNbtString)
            component = component with { Formatting = component.Formatting with { Color = TextColor.FromString(colorNbtString.Value) } };

        if (TryGetAs<NbtString>(compound, "font") is { } fontNbtString)
            component = component with { Formatting = component.Formatting with { Font = fontNbtString.Value } };

        if (TryGetAs<NbtByte>(compound, "bold") is { } boldNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsBold = boldNbtBoolean.IsTrue } };

        if (TryGetAs<NbtByte>(compound, "italic") is { } italicNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsItalic = italicNbtBoolean.IsTrue } };

        if (TryGetAs<NbtByte>(compound, "underlined") is { } underlinedNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsUnderlined = underlinedNbtBoolean.IsTrue } };

        if (TryGetAs<NbtByte>(compound, "strikethrough") is { } strikethroughNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsStrikethrough = strikethroughNbtBoolean.IsTrue } };

        if (TryGetAs<NbtByte>(compound, "obfuscated") is { } obfuscatedNbtBoolean)
            component = component with { Formatting = component.Formatting with { IsObfuscated = obfuscatedNbtBoolean.IsTrue } };

        if (TryGetAs<NbtTag>(compound, "shadow_color") is { } shadowColorNbtTag)
        {
            if (shadowColorNbtTag is NbtList { DataType: NbtTagType.Float } shadowColorNbtList)
                component = component with { Formatting = component.Formatting with { ShadowColor = shadowColorNbtList.Data.Select(dataTag => ((NbtFloat)dataTag).Value).ToArray() } };
            else if (shadowColorNbtTag is NbtInt shadowColorNbtInt)
                component = component with { Formatting = component.Formatting with { ShadowColor = shadowColorNbtInt.Value } };
        }
    }

    private static void DeserializeInteractivity(ref Component component, NbtTag tag)
    {
        var compound = tag.AsCompound();

        if (TryGetAs<NbtString>(compound, "insertion") is { } insertionNbtString)
            component = component with { Interactivity = component.Interactivity with { Insertion = insertionNbtString.Value } };

        if (TryGetAs<NbtCompound>(compound, "click_event") is { } clickEventNbtCompound)
        {
            var actionNbtString = GetAs<NbtString>(clickEventNbtCompound, "action");

            var action = actionNbtString.Value switch
            {
                "open_url" => new OpenUrl(GetAs<NbtString>(clickEventNbtCompound, "url").Value) as IClickEventAction,
                "open_file" => new OpenFile(GetAs<NbtString>(clickEventNbtCompound, "value").Value),
                "run_command" => new RunCommand(GetAs<NbtString>(clickEventNbtCompound, "command").Value),
                "suggest_command" => new SuggestCommand(GetAs<NbtString>(clickEventNbtCompound, "command").Value),
                "change_page" => new ChangePage(GetAs<NbtInt>(clickEventNbtCompound, "page").Value),
                "copy_to_clipboard" => new CopyToClipboard(GetAs<NbtString>(clickEventNbtCompound, "value").Value),
                var value => throw new NotSupportedException(value)
            };

            component = component with { Interactivity = component.Interactivity with { ClickEvent = new ClickEvent(action) } };
        }

        if (TryGetAs<NbtCompound>(compound, "hover_event") is { } hoverEventNbtCompound)
        {
            var actionNbtString = GetAs<NbtString>(hoverEventNbtCompound, "action");

            var content = actionNbtString.Value switch
            {
                "show_text" => GetAs<NbtTag>(hoverEventNbtCompound, "value") switch
                {
                    NbtString value => new ShowText(Deserialize(value)),
                    NbtCompound value => new ShowText(Deserialize(value)),
                    var value => throw new NbtException(value)
                } as IHoverEventAction,
                "show_item" => new ShowItem(GetAs<NbtString>(hoverEventNbtCompound, "id").Value, TryGetAs<NbtInt>(hoverEventNbtCompound, "type")?.Value, TryGetAs<NbtCompound>(hoverEventNbtCompound, "components")),
                "show_entity" => new ShowEntity(GetAs<NbtTag>(hoverEventNbtCompound, "uuid") switch
                {
                    NbtString idNbtString => Uuid.Parse(idNbtString.Value),
                    NbtIntArray idNbtIntArray => Uuid.Parse([.. idNbtIntArray.Data]),
                    var value => throw new NbtException(value)
                },
                TryGetAs<NbtString>(hoverEventNbtCompound, "id")?.Value,
                TryGetAs<NbtTag>(hoverEventNbtCompound, "name") switch
                {
                    { } nameTag => Deserialize(nameTag),
                    _ => null
                }),
                var value => throw new NotSupportedException(value)
            };

            component = component with { Interactivity = component.Interactivity with { HoverEvent = new HoverEvent(content) } };
        }
    }
}
