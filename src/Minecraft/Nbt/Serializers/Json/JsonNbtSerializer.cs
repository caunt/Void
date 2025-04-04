﻿using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json.Tags;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Nbt.Serializers.Json;

public static class JsonNbtSerializer
{
    public static readonly JsonSerializerOptions Options = new();

    static JsonNbtSerializer()
    {
        Options.Converters.Add(new NbtTagJsonConverter());
        Options.Converters.Add(new NbtTagTypeJsonConverter());

        Options.Converters.Add(new NbtBooleanJsonConverter());
        Options.Converters.Add(new NbtByteJsonConverter());
        Options.Converters.Add(new NbtByteArrayJsonConverter());
        Options.Converters.Add(new NbtCompoundJsonConverter());
        Options.Converters.Add(new NbtDoubleJsonConverter());
        Options.Converters.Add(new NbtEndJsonConverter());
        Options.Converters.Add(new NbtFloatJsonConverter());
        Options.Converters.Add(new NbtIntJsonConverter());
        Options.Converters.Add(new NbtIntArrayJsonConverter());
        Options.Converters.Add(new NbtListJsonConverter());
        Options.Converters.Add(new NbtLongJsonConverter());
        Options.Converters.Add(new NbtLongArrayJsonConverter());
        Options.Converters.Add(new NbtShortJsonConverter());
        Options.Converters.Add(new NbtStringJsonConverter());

        Options.Converters.Add(new UuidJsonConverter());
    }

    public static JsonNode Serialize(NbtTag tag)
    {
        return JsonSerializer.SerializeToNode(tag, Options) ?? throw new JsonException("Nbt cannot be serialized to json.");
    }

    public static NbtTag Deserialize(JsonNode node)
    {
        return JsonSerializer.Deserialize<NbtTag>(node, Options) ?? throw new JsonException("Nbt cannot be deserialized from json.");
    }
}
