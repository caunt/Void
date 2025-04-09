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

namespace Void.Minecraft.Components.Text.Transformers;

public static class ComponentNbtTransformers
{
    public static NamedNbtProperty Apply(NamedNbtProperty property, ProtocolVersion from, ProtocolVersion to)
    {
        return NamedNbtProperty.FromNbtTag(Apply(property.AsNbtTag, from, to));
    }

    public static NbtProperty Apply(NbtProperty property, ProtocolVersion from, ProtocolVersion to)
    {
        return NbtProperty.FromNbtTag(Apply(property.AsNbtTag, from, to));
    }

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

    private static NbtTag Downgrade(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (from > ProtocolVersion.MINECRAFT_1_20_2 && to <= ProtocolVersion.MINECRAFT_1_20_2)
            tag = Downgrade_v1_20_3_to_v1_20_2(tag, from, to);

        if (from > ProtocolVersion.MINECRAFT_1_15_2 && to <= ProtocolVersion.MINECRAFT_1_15_2)
            tag = Downgrade_v1_16_to_v1_15_2(tag, from, to);

        return tag;
    }

    private static NbtTag Upgrade(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (from <= ProtocolVersion.MINECRAFT_1_15_2 && to > ProtocolVersion.MINECRAFT_1_15_2)
            tag = Upgrade_v1_15_2_to_v1_16(tag, from, to);

        if (from <= ProtocolVersion.MINECRAFT_1_20_2 && to > ProtocolVersion.MINECRAFT_1_20_2)
            tag = Upgrade_v1_20_2_to_v1_20_3(tag, from, to);

        return tag;
    }

    private static NbtTag Downgrade_v1_20_3_to_v1_20_2(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        Console.WriteLine("Nbt Downgrade_v1_20_3_to_v1_20_2 not supported");
        return tag;
    }

    private static NbtTag Downgrade_v1_16_to_v1_15_2(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (tag is NbtCompound root)
        {
            if (root["color"] is NbtString colorTag)
            {
                var color = TextColor.FromString(colorTag.Value);
                var downsampled = color.Downsample();

                root["color"] = new NbtString(downsampled.Name);
            }

            if (root["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["contents"] is NbtCompound contentsCompound)
                {
                    hoverEvent.Values.Remove("contents");

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

            if (root["with"] is NbtList with)
                root["with"] = new NbtList(with.Data.Select(childTag => Downgrade_v1_16_to_v1_15_2(childTag, from, to)), with.DataType);

            if (root["extra"] is NbtList extra)
                root["extra"] = new NbtList(extra.Data.Select(childTag => Downgrade_v1_16_to_v1_15_2(childTag, from, to)), extra.DataType);
        }

        return tag;
    }

    private static NbtTag Upgrade_v1_20_2_to_v1_20_3(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        Console.WriteLine("Nbt Upgrade_v1_20_2_to_v1_20_3 not supported");
        return tag;
    }

    private static NbtTag Upgrade_v1_15_2_to_v1_16(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (tag is NbtCompound root)
        {
            if (root["hoverEvent"] is NbtCompound hoverEvent)
            {
                if (hoverEvent["value"] is NbtTag value)
                {
                    hoverEvent.Values.Remove("value");

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

                            contents = NbtUnsafeStringSerializer.Deserialize(textValue.Value);
                        }

                        hoverEvent["contents"] = contents;
                    }
                }

                if (root["with"] is NbtList with)
                    root["with"] = new NbtList(with.Data.Select(childTag => Upgrade_v1_15_2_to_v1_16(childTag, from, to)), with.DataType);

                if (root["extra"] is NbtList extra)
                    root["extra"] = new NbtList(extra.Data.Select(childTag => Upgrade_v1_15_2_to_v1_16(childTag, from, to)), extra.DataType);
            }
        }

        return tag;
    }
}
