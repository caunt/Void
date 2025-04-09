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
            tag = Downgrade_v1_20_3_to_v1_20_2(tag);

        if (from > ProtocolVersion.MINECRAFT_1_15_2 && to <= ProtocolVersion.MINECRAFT_1_15_2)
            tag = Downgrade_v1_16_to_v1_15_2(tag);

        return tag;
    }

    private static NbtTag Upgrade(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
    {
        if (from <= ProtocolVersion.MINECRAFT_1_15_2 && to > ProtocolVersion.MINECRAFT_1_15_2)
            tag = Upgrade_v1_15_2_to_v1_16(tag);

        if (from <= ProtocolVersion.MINECRAFT_1_20_2 && to > ProtocolVersion.MINECRAFT_1_20_2)
            tag = Upgrade_v1_20_2_to_v1_20_3(tag);

        return tag;
    }

    public static void Passthrough_v1_20_3_to_v1_20_2(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NamedNbtProperty>();
        var tag = Downgrade_v1_20_3_to_v1_20_2(property.AsNbtTag);

        property = NamedNbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    public static void Passthrough_v1_16_to_v1_15_2(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NamedNbtProperty>();
        var tag = Downgrade_v1_16_to_v1_15_2(property.AsNbtTag);

        property = NamedNbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    public static void Passthrough_v1_20_2_to_v1_20_3(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NamedNbtProperty>();
        var tag = Upgrade_v1_20_2_to_v1_20_3(property.AsNbtTag);

        property = NamedNbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

    public static void Passthrough_v1_15_2_to_v1_16(IMinecraftBinaryPacketWrapper wrapper)
    {
        var property = wrapper.Read<NamedNbtProperty>();
        var tag = Upgrade_v1_15_2_to_v1_16(property.AsNbtTag);

        property = NamedNbtProperty.FromNbtTag(tag);
        wrapper.Write(property);
    }

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
                                contentsCompound["id"] = new NbtString(Uuid.Parse([.. idIntArray.Data]).ToString());
                            else if (contentsCompound["id"] is not NbtString)
                                throw new NotSupportedException($"Non-string id value found: {contentsCompound["id"]}");
                        }
                    }
                }
            }
        }

        return tag;
    }

    public static NbtTag Downgrade_v1_16_to_v1_15_2(NbtTag tag)
    {
        if (tag is NbtCompound rootCompound)
        {
            // Downsample colors to list of compatible
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

    public static NbtTag Upgrade_v1_20_2_to_v1_20_3(NbtTag tag)
    {
        Console.WriteLine("Nbt Upgrade_v1_20_2_to_v1_20_3 not supported");
        return tag;
    }

    public static NbtTag Upgrade_v1_15_2_to_v1_16(NbtTag tag)
    {
        if (tag is NbtCompound root)
        {
            // Replace the "value" field with "contents" field
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

        return tag;
    }
}
