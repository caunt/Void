using System;
using System.Linq;
using System.Text.Json.Nodes;
using Void.Minecraft.Components.Text.Colors;
using Void.Minecraft.Nbt;
using Void.Minecraft.Nbt.Serializers.String;
using Void.Minecraft.Nbt.Tags;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Transformers;

/// <summary>Applies version-specific schema migrations to NBT text components.</summary>
public static class ComponentNbtTransformers
{
    private const string ShowAchievementMarker = "!1.11.1=>1.12!";

    /// <summary>Reads, migrates, and rewrites one NBT component property using the naming convention required by the endpoint versions.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    /// <param name="from">The source protocol version.</param>
    /// <param name="to">The destination protocol version.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Apply(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion from, ProtocolVersion to)
    {
        if (from <= ProtocolVersion.MINECRAFT_1_20 && to > ProtocolVersion.MINECRAFT_1_20)
        {
            var namedNbtProperty = wrapper.Read<NamedNbtProperty>();
            namedNbtProperty = Apply(namedNbtProperty, from, to);
            wrapper.Write(NbtProperty.FromNbtTag(namedNbtProperty.AsNbtTag));
        }
        else if (from > ProtocolVersion.MINECRAFT_1_20 && to <= ProtocolVersion.MINECRAFT_1_20)
        {
            var nbtProperty = wrapper.Read<NbtProperty>();
            nbtProperty = Apply(nbtProperty, from, to);
            wrapper.Write(NamedNbtProperty.FromNbtTag(nbtProperty.AsNbtTag));
        }
        else if (from > ProtocolVersion.MINECRAFT_1_20 && to > ProtocolVersion.MINECRAFT_1_20)
        {
            var nbtProperty = wrapper.Read<NbtProperty>();
            nbtProperty = Apply(nbtProperty, from, to);
            wrapper.Write(nbtProperty);
        }
        else
        {
            var namedNbtProperty = wrapper.Read<NamedNbtProperty>();
            namedNbtProperty = Apply(namedNbtProperty, from, to);
            wrapper.Write(namedNbtProperty);
        }
    }

    /// <summary>Migrates the tag contained in a named NBT property.</summary>
    /// <param name="property">The source property.</param>
    /// <param name="from">The source protocol version.</param>
    /// <param name="to">The destination protocol version.</param>
    /// <returns>A named property containing the migrated tag.</returns>
    [Obsolete("Rewrite properties yourself instead.")]
    public static NamedNbtProperty Apply(NamedNbtProperty property, ProtocolVersion from, ProtocolVersion to)
    {
        return NamedNbtProperty.FromNbtTag(Apply(property.AsNbtTag, from, to));
    }

    /// <summary>Migrates the tag contained in an unnamed NBT property.</summary>
    /// <param name="property">The source property.</param>
    /// <param name="from">The source protocol version.</param>
    /// <param name="to">The destination protocol version.</param>
    /// <returns>An unnamed property containing the migrated tag.</returns>
    [Obsolete("Rewrite properties yourself instead.")]
    public static NbtProperty Apply(NbtProperty property, ProtocolVersion from, ProtocolVersion to)
    {
        return NbtProperty.FromNbtTag(Apply(property.AsNbtTag, from, to));
    }

    /// <summary>Applies every component-schema transition crossed by a protocol-version change.</summary>
    /// <param name="tag">The component tag. Compound graphs may be mutated in place.</param>
    /// <param name="from">The source protocol version.</param>
    /// <param name="to">The destination protocol version.</param>
    /// <returns>The migrated tag. JSON stored in a string tag is migrated when it can be parsed.</returns>
    [Obsolete("Rewrite properties yourself instead.")]
    public static NbtTag Apply(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (tag is NbtString nbtString)
        {
            var node = (JsonNode?)null;

            try
            {
                node = JsonNode.Parse(nbtString.Value);
            }
            catch
            {
                // Ignore, not a json
            }

            if (node is null)
                return nbtString;

            return new NbtString(ComponentJsonTransformers.Apply(node, from, to).ToString());
        }

        var type = from > to ? TransformationType.Downgrade : TransformationType.Upgrade;

        return type switch
        {
            TransformationType.Downgrade => Downgrade(tag, from, to),
            TransformationType.Upgrade => Upgrade(tag, from, to),
            _ => tag
        };
    }

    [Obsolete("Rewrite properties yourself instead.")]
    private static NbtTag Downgrade(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (from > ProtocolVersion.MINECRAFT_1_21_4 && to <= ProtocolVersion.MINECRAFT_1_21_4)
            tag = Downgrade_v1_21_5_to_v1_21_4(tag);

        if (from > ProtocolVersion.MINECRAFT_1_20_2 && to <= ProtocolVersion.MINECRAFT_1_20_2)
            tag = Downgrade_v1_20_3_to_v1_20_2(tag);

        if (from > ProtocolVersion.MINECRAFT_1_15_2 && to <= ProtocolVersion.MINECRAFT_1_15_2)
            tag = Downgrade_v1_16_to_v1_15_2(tag);

        if (from > ProtocolVersion.MINECRAFT_1_11_1 && to <= ProtocolVersion.MINECRAFT_1_11_1)
            tag = Downgrade_v1_12_to_v1_11_1(tag);

        if (from > ProtocolVersion.MINECRAFT_1_8 && to <= ProtocolVersion.MINECRAFT_1_8)
            tag = Downgrade_v1_9_to_v1_8(tag);

        return tag;
    }

    [Obsolete("Rewrite properties yourself instead.")]
    private static NbtTag Upgrade(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (from <= ProtocolVersion.MINECRAFT_1_8 && to > ProtocolVersion.MINECRAFT_1_8)
            tag = Upgrade_v1_8_to_v1_9(tag);

        if (from <= ProtocolVersion.MINECRAFT_1_11_1 && to > ProtocolVersion.MINECRAFT_1_11_1)
            tag = Upgrade_v1_11_1_to_v1_12(tag);

        if (from <= ProtocolVersion.MINECRAFT_1_15_2 && to > ProtocolVersion.MINECRAFT_1_15_2)
            tag = Upgrade_v1_15_2_to_v1_16(tag);

        if (from <= ProtocolVersion.MINECRAFT_1_20_2 && to > ProtocolVersion.MINECRAFT_1_20_2)
            tag = Upgrade_v1_20_2_to_v1_20_3(tag);

        if (from <= ProtocolVersion.MINECRAFT_1_21_4 && to > ProtocolVersion.MINECRAFT_1_21_4)
            tag = Upgrade_v1_21_4_to_v1_21_5(tag);

        return tag;
    }

    #region Downgrade
    /// <summary>Reads, downgrades, and rewrites one NBT component property from 1.21.5 to 1.21.4 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_21_5_to_v1_21_4(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Downgrade_v1_21_5_to_v1_21_4(property.AsNbtTag);

        wrapper.Write(NbtProperty.FromNbtTag(tag));
    }

    /// <summary>Reads, downgrades, and rewrites one NBT component property from 1.20.3 to 1.20.2 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_20_3_to_v1_20_2(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Downgrade_v1_20_3_to_v1_20_2(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    /// <summary>Reads, downgrades, and rewrites one NBT component property from 1.16 to 1.15.2 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_16_to_v1_15_2(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Downgrade_v1_16_to_v1_15_2(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    /// <summary>Reads, downgrades, and rewrites one NBT component property from 1.12 to 1.11.1 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_12_to_v1_11_1(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Downgrade_v1_12_to_v1_11_1(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    /// <summary>Reads, downgrades, and rewrites one NBT component property from 1.9 to 1.8 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_9_to_v1_8(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Downgrade_v1_9_to_v1_8(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }
    #endregion

    #region Upgrade
    /// <summary>Reads, upgrades, and rewrites one NBT component property from 1.21.4 to 1.21.5 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_21_4_to_v1_21_5(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Upgrade_v1_21_4_to_v1_21_5(property.AsNbtTag);

        wrapper.Write(NbtProperty.FromNbtTag(tag));
    }

    /// <summary>Reads, upgrades, and rewrites one NBT component property from 1.20.2 to 1.20.3 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_20_2_to_v1_20_3(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Upgrade_v1_20_2_to_v1_20_3(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    /// <summary>Reads, upgrades, and rewrites one NBT component property from 1.15.2 to 1.16 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_15_2_to_v1_16(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Upgrade_v1_15_2_to_v1_16(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    /// <summary>Reads, upgrades, and rewrites one NBT component property from 1.11.1 to 1.12 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_11_1_to_v1_12(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Upgrade_v1_11_1_to_v1_12(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    /// <summary>Reads, upgrades, and rewrites one NBT component property from 1.8 to 1.9 format.</summary>
    /// <param name="wrapper">The packet wrapper whose cursor is advanced by one property.</param>
    [Obsolete("Rewrite properties yourself instead.")]
    public static void Passthrough_v1_8_to_v1_9(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NbtProperty>();
        var tag = Upgrade_v1_8_to_v1_9(property.AsNbtTag);

        property = NbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }
    #endregion

    /// <summary>Converts snake-case 1.21.5 interaction fields to the nested 1.21.4 component schema.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    public static NbtTag Downgrade_v1_21_5_to_v1_21_4(NbtTag tag)
    {
        if (tag is NbtCompound root)
        {
            if (root["hover_event"] is NbtCompound hoverEvent)
            {
                root.RenameKey("hover_event", "hoverEvent");

                if (hoverEvent["action"] is NbtString action)
                {
                    switch (action.Value)
                    {
                        case "show_text":
                            hoverEvent.RenameKey("value", "contents");
                            break;
                        case "show_item":
                            var contents = new NbtCompound();

                            foreach (var (key, value) in hoverEvent.Fields)
                            {
                                if (key is "action")
                                    continue;

                                if (key == "id")
                                {
                                    hoverEvent.RenameKey("id", "contents");
                                    break;
                                }
                                else
                                {
                                    contents[key] = value;
                                }
                            }

                            if (contents.Fields.Count > 0)
                                hoverEvent["contents"] = contents;
                            break;
                        case "show_entity":
                            var entityContents = new NbtCompound();
                            foreach (var (key, value) in hoverEvent.Fields)
                            {
                                if (key is "action")
                                    continue;

                                if (key == "uuid")
                                    entityContents["id"] = value;
                                else if (key == "id")
                                    entityContents["type"] = value;
                                else
                                    entityContents[key] = value;
                            }

                            hoverEvent["contents"] = entityContents;
                            break;
                    }
                }
            }

            if (root["click_event"] is NbtCompound clickEvent)
            {
                root.RenameKey("click_event", "clickEvent");

                if (clickEvent["action"] is NbtString action)
                {
                    switch (action.Value)
                    {
                        case "open_url":
                            clickEvent.RenameKey("url", "value");
                            break;
                        case "run_command" or "suggest_command":
                            clickEvent.RenameKey("command", "value");
                            break;
                        case "change_page" when clickEvent["page"] is NbtInt page:
                            clickEvent.RenameKey("page", "value");
                            break;
                    }
                }
            }

            // Replace recursive text components
            if (root["with"] is NbtList with)
                root["with"] = new NbtList(with.Data.Select(Downgrade_v1_21_5_to_v1_21_4), with.DataType);

            if (root["extra"] is NbtList extra)
                root["extra"] = new NbtList(extra.Data.Select(Downgrade_v1_21_5_to_v1_21_4), extra.DataType);
        }

        return tag;
    }

    /// <summary>Converts entity-hover UUID integer arrays to strings for 1.20.2.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    /// <exception cref="NotSupportedException">An entity-hover identifier is neither an integer array nor a string.</exception>
    public static NbtTag Downgrade_v1_20_3_to_v1_20_2(NbtTag tag)
    {
        // Replace "show_entity" contents "id" int array value to string
        if (tag is NbtCompound rootCompound)
        {
            if (rootCompound["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["contents"] is NbtCompound contentsCompound)
                {
                    if (contentsCompound["action"] is NbtString action)
                    {
                        if (action.Value is "show_entity")
                        {
                            if (contentsCompound["id"] is NbtIntArray idIntArray)
                                contentsCompound["id"] = new NbtString(Uuid.FromIntArray(idIntArray.Data).ToString());
                            else if (contentsCompound["id"] is not NbtString)
                                throw new NotSupportedException($"Non-string id value found: {contentsCompound["id"]}");
                        }
                    }
                }
            }

            // Replace recursive text components
            if (rootCompound["with"] is NbtList with)
                rootCompound["with"] = new NbtList(with.Data.Select(Downgrade_v1_20_3_to_v1_20_2), with.DataType);

            if (rootCompound["extra"] is NbtList extra)
                rootCompound["extra"] = new NbtList(extra.Data.Select(Downgrade_v1_20_3_to_v1_20_2), extra.DataType);
        }

        return tag;
    }

    /// <summary>Downsamples colors and converts hover contents to values for 1.15.2.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag; compact string components are expanded to compounds.</returns>
    public static NbtTag Downgrade_v1_16_to_v1_15_2(NbtTag tag)
    {
        if (tag is NbtCompound rootCompound)
        {
            // Downsample colors to a list of compatible colors
            if (rootCompound["color"] is NbtString colorTag)
            {
                var color = TextColor.FromString(colorTag.Value);
                var downsampled = color.Downsample();

                rootCompound["color"] = new NbtString(downsampled.Name);
            }

            // Replace the "contents" field with "value" field
            if (rootCompound["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["contents"] is NbtCompound contentsCompound)
                {
                    hoverEvent.Fields.Remove("contents");

                    if (contentsCompound["action"] is NbtString action)
                    {
                        if (action.Value is "show_text" or "show_achievement")
                        {
                            hoverEvent["value"] = contentsCompound;
                        }
                        else if (action.Value is "show_item" or "show_entity")
                        {
                            hoverEvent["value"] = new NbtString(contentsCompound.ToString());
                        }
                    }
                }
            }

            // Replace recursive text components
            if (rootCompound["with"] is NbtList with)
                rootCompound["with"] = new NbtList(with.Data.Select(Downgrade_v1_16_to_v1_15_2), with.DataType);

            if (rootCompound["extra"] is NbtList extra)
                rootCompound["extra"] = new NbtList(extra.Data.Select(Downgrade_v1_16_to_v1_15_2), extra.DataType);
        }

        // De-compact text component
        if (tag is NbtString rootString)
            tag = new NbtCompound { ["text"] = rootString };

        return tag;
    }

    /// <summary>Restores marked achievement hover events when downgrading to 1.11.1.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    public static NbtTag Downgrade_v1_12_to_v1_11_1(NbtTag tag)
    {
        if (tag is NbtCompound rootCompound)
        {
            // Replace the "show_text" action back to "show_achievement"
            if (rootCompound["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["value"] is { } value)
                {
                    if (hoverEvent["action"] is { } action)
                    {
                        if (value is NbtString valueString)
                        {
                            if (valueString.Value.StartsWith(ShowAchievementMarker))
                            {
                                if (action is NbtString { Value: "show_text" })
                                {
                                    hoverEvent["action"] = new NbtString("show_achievement");
                                    hoverEvent["value"] = new NbtCompound { ["text"] = new NbtString(valueString.Value[ShowAchievementMarker.Length..]) };
                                }
                            }
                        }
                    }
                }
            }

            // Replace recursive text components
            if (rootCompound["with"] is NbtList with)
                rootCompound["with"] = new NbtList(with.Data.Select(Downgrade_v1_12_to_v1_11_1), with.DataType);

            if (rootCompound["extra"] is NbtList extra)
                rootCompound["extra"] = new NbtList(extra.Data.Select(Downgrade_v1_12_to_v1_11_1), extra.DataType);
        }

        return tag;
    }

    /// <summary>Converts structured legacy hover values to scalar values for 1.8.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    /// <exception cref="NotSupportedException">A supported structured hover action has no text value.</exception>
    public static NbtTag Downgrade_v1_9_to_v1_8(NbtTag tag)
    {
        if (tag is NbtCompound rootCompound)
        {
            // Replace the "show_text" action back to "show_achievement"
            if (rootCompound["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["value"] is { } value)
                {
                    if (hoverEvent["action"] is NbtString { } action)
                    {
                        if (value is NbtCompound valueCompound)
                        {
                            if (action.Value is "show_achievement" or "show_entity" or "show_item")
                            {
                                if (valueCompound["text"] is not { } text)
                                    throw new NotSupportedException($"Text value not found: {value}");

                                hoverEvent["value"] = text;
                            }
                        }
                    }
                }
            }

            // Replace recursive text components
            if (rootCompound["with"] is NbtList with)
                rootCompound["with"] = new NbtList(with.Data.Select(Downgrade_v1_9_to_v1_8), with.DataType);

            if (rootCompound["extra"] is NbtList extra)
                rootCompound["extra"] = new NbtList(extra.Data.Select(Downgrade_v1_9_to_v1_8), extra.DataType);
        }

        return tag;
    }

    /// <summary>Converts nested 1.21.4 interaction fields to the flattened snake-case 1.21.5 schema.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    public static NbtTag Upgrade_v1_21_4_to_v1_21_5(NbtTag tag)
    {
        if (tag is NbtCompound root)
        {
            if (root["hoverEvent"] is NbtCompound hoverEvent)
            {
                root.RenameKey("hoverEvent", "hover_event");

                if (hoverEvent["action"] is NbtString action)
                {
                    switch (action.Value)
                    {
                        case "show_text":
                            hoverEvent.RenameKey("contents", "value");
                            break;
                        case "show_item" when hoverEvent["contents"] is NbtString contents:
                            hoverEvent["id"] = contents;
                            break;
                        case "show_item" when hoverEvent["contents"] is NbtCompound contents:
                            foreach (var (key, value) in contents.Fields)
                                hoverEvent[key] = value;
                            break;
                        case "show_entity" when hoverEvent["contents"] is NbtCompound contents:
                            foreach (var (key, value) in contents.Fields)
                            {
                                var newKey = key switch
                                {
                                    "id" => "uuid",
                                    "type" => "id",
                                    _ => key
                                };

                                hoverEvent[newKey] = value;
                            }
                            break;
                    }
                }
            }

            if (root["clickEvent"] is NbtCompound clickEvent)
            {
                root.RenameKey("clickEvent", "click_event");

                if (clickEvent["action"] is NbtString action)
                {
                    switch (action.Value)
                    {
                        case "open_url":
                            clickEvent.RenameKey("value", "url");
                            break;
                        case "run_command" or "suggest_command":
                            clickEvent.RenameKey("value", "command");
                            break;
                        case "change_page" when clickEvent["value"] is NbtString page:
                            clickEvent["page"] = new NbtInt(int.Parse(page.Value));
                            break;
                    }
                }
            }

            // Replace recursive text components
            if (root["with"] is NbtList with)
                root["with"] = new NbtList(with.Data.Select(Upgrade_v1_21_4_to_v1_21_5), with.DataType);

            if (root["extra"] is NbtList extra)
                root["extra"] = new NbtList(extra.Data.Select(Upgrade_v1_21_4_to_v1_21_5), extra.DataType);
        }

        return tag;
    }

    /// <summary>Applies the component representation transition from 1.20.2 to 1.20.3.</summary>
    /// <param name="tag">The component tag to migrate.</param>
    /// <returns>The migrated tag.</returns>
    public static NbtTag Upgrade_v1_20_2_to_v1_20_3(NbtTag tag)
    {
        return tag;
    }

    /// <summary>Upgrades hover values and compact strings to the 1.16 NBT component schema.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    /// <exception cref="NotSupportedException">An item or entity hover value has no SNBT text field.</exception>
    public static NbtTag Upgrade_v1_15_2_to_v1_16(NbtTag tag)
    {
        if (tag is NbtCompound root)
        {
            // Replace the "value" field with "contents" field
            if (root["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["value"] is NbtTag value)
                {
                    hoverEvent.Fields.Remove("value");

                    if (hoverEvent["action"] is NbtString action)
                    {
                        var contents = (NbtTag?)null;

                        if (action.Value is "show_text" or "show_achievement")
                        {
                            contents = value;
                        }
                        else if (action.Value is "show_item" or "show_entity")
                        {
                            if (value is not NbtCompound compoundValue)
                                throw new NotSupportedException($"Non-compound value found: {value}");

                            if (compoundValue["text"] is not { } text)
                                throw new NotSupportedException("Text in value not found");

                            if (text is not NbtString textValue)
                                throw new NotSupportedException($"Non-string text value found: {text}");

                            contents = NbtStringSerializer.Deserialize(textValue.Value);
                        }

                        hoverEvent["contents"] = contents;
                    }
                }

                // Replace recursive text components
                if (root["with"] is NbtList with)
                    root["with"] = new NbtList(with.Data.Select(Upgrade_v1_15_2_to_v1_16), with.DataType);

                if (root["extra"] is NbtList extra)
                    root["extra"] = new NbtList(extra.Data.Select(Upgrade_v1_15_2_to_v1_16), extra.DataType);
            }
        }

        // De-compact text component
        if (tag is NbtString rootString)
            tag = new NbtCompound { ["text"] = rootString };

        return tag;
    }

    /// <summary>Converts achievement hover events to marked text hover events for 1.12.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    public static NbtTag Upgrade_v1_11_1_to_v1_12(NbtTag tag)
    {
        if (tag is NbtCompound root)
        {
            // Replace the "show_achievement" action with "show_text"
            if (root["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["value"] is NbtCompound { } value)
                {
                    if (hoverEvent["action"] is { } action)
                    {
                        if (action is NbtString { Value: "show_achievement" })
                        {
                            hoverEvent["action"] = new NbtString("show_text");

                            if (value["text"] is not NbtString { } text)
                                throw new NotSupportedException($"Text value not found: {value}");

                            hoverEvent["value"] = new NbtString(ShowAchievementMarker + text.Value);
                        }
                    }
                }
            }

            // Replace recursive text components
            if (root["with"] is NbtList with)
                root["with"] = new NbtList(with.Data.Select(Upgrade_v1_11_1_to_v1_12), with.DataType);

            if (root["extra"] is NbtList extra)
                root["extra"] = new NbtList(extra.Data.Select(Upgrade_v1_11_1_to_v1_12), extra.DataType);
        }

        return tag;
    }

    /// <summary>Wraps supported scalar hover values in text compounds for 1.9.</summary>
    /// <param name="tag">The component tag to mutate recursively.</param>
    /// <returns>The migrated tag.</returns>
    public static NbtTag Upgrade_v1_8_to_v1_9(NbtTag tag)
    {
        if (tag is NbtCompound root)
        {
            if (root["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["value"] is { } value)
                {
                    if (hoverEvent["action"] is NbtString { } action)
                    {
                        if (action.Value is "show_achievement" or "show_entity" or "show_item")
                        {
                            if (value is NbtCompound valueAsCompound && valueAsCompound.ContainsKey("text"))
                                hoverEvent["value"] = value; // Minecraft Console Clients sends NbtCompound as value
                            else
                                hoverEvent["value"] = new NbtCompound { ["text"] = value }; // Vanilla behavior
                        }
                    }
                }
            }

            // Replace recursive text components
            if (root["with"] is NbtList with)
                root["with"] = new NbtList(with.Data.Select(Upgrade_v1_8_to_v1_9), with.DataType);

            if (root["extra"] is NbtList extra)
                root["extra"] = new NbtList(extra.Data.Select(Upgrade_v1_8_to_v1_9), extra.DataType);
        }

        return tag;
    }
}
