﻿using SharpNBT.SNBT;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;
using Void.Minecraft.Components.Text.Serializers;
using Void.Minecraft.Nbt;
using Void.Minecraft.Network;

namespace Void.Minecraft.Components.Text;

public record Component(IContent Content, Children Children, Formatting Formatting, Interactivity Interactivity)
{
    public static Component Default { get; } = new(new TextContent(string.Empty), Children.Default, Formatting.Default, Interactivity.Default);

    public static implicit operator Component(string text) => DeserializeLegacy(text);

    public static Component ReadFrom(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
        {
            var value = buffer.ReadString();
            var node = (JsonNode?)null;

            try
            {
                node = JsonNode.Parse(value);
            }
            catch (Exception exception) when (exception is JsonException)
            {
                // ignore, not a json
            }

            if (node is null)
                return DeserializeLegacy(value);
            else
                return DeserializeJson(node, protocolVersion);
        }
        else
        {
            return DeserializeNbt(buffer.ReadTag(), protocolVersion);
        }
    }

    public void WriteTo(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion <= ProtocolVersion.MINECRAFT_1_20_2)
            buffer.WriteString(SerializeJson(protocolVersion).ToString());
        else
            buffer.WriteTag(SerializeNbt(protocolVersion));
    }

    public static Component DeserializeLegacy(string source, char prefix = '&')
    {
        return LegacyComponentSerializer.Deserialize(source, prefix);
    }

    public static Component DeserializeJson(JsonNode source, ProtocolVersion protocolVersion)
    {
        return JsonComponentSerializer.Deserialize(source, protocolVersion);
    }

    public static Component DeserializeNbt(NbtTag tag, ProtocolVersion protocolVersion)
    {
        return NbtComponentSerializer.Deserialize(tag, protocolVersion);
    }

    public static Component DeserializeSnbt(string source, ProtocolVersion protocolVersion)
    {
        return DeserializeNbt(StringNbt.Parse(source), protocolVersion);
    }

    public string SerializeLegacy(char prefix = '&')
    {
        return LegacyComponentSerializer.Serialize(this, prefix);
    }

    public JsonNode SerializeJson(ProtocolVersion protocolVersion)
    {
        return JsonComponentSerializer.Serialize(this, protocolVersion);
    }

    public NbtTag SerializeNbt(ProtocolVersion protocolVersion)
    {
        return NbtComponentSerializer.Serialize(this, protocolVersion);
    }

    public string SerializeSnbt(ProtocolVersion protocolVersion)
    {
        return SerializeNbt(protocolVersion).ToString();
    }

    public override string ToString() => SerializeNbt(ProtocolVersion.Latest).ToString();
}