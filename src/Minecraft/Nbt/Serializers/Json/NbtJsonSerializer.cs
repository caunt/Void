using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json.Tags;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Nbt.Serializers.Json;

/// <summary>Serializes NBT tags as natural JSON values and infers tag types while deserializing.</summary>
public static class NbtJsonSerializer
{
    /// <summary>Gets the shared mutable serializer options containing all NBT and UUID converters.</summary>
    public static readonly JsonSerializerOptions Options = new();

    static NbtJsonSerializer()
    {
        Options.Converters.Add(new NbtTagJsonConverter());
        Options.Converters.Add(new NbtTagTypeJsonConverter());

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

    /// <summary>Serializes an NBT tag to a JSON node.</summary>
    /// <param name="tag">The tag to serialize.</param>
    /// <returns>The JSON representation.</returns>
    /// <exception cref="JsonException">Serialization produces no JSON node.</exception>
    public static JsonNode Serialize(NbtTag tag)
    {
        return JsonSerializer.SerializeToNode(tag, Options) ?? throw new JsonException("Nbt cannot be serialized to JSON.");
    }

    /// <summary>Deserializes JSON text, falling back to an NBT string when the input is not valid JSON.</summary>
    /// <param name="value">The JSON text or literal string value.</param>
    /// <returns>The inferred NBT tag.</returns>
    public static NbtTag Deserialize(string value)
    {
        var node = (JsonNode?)null;

        try
        {
            node = JsonNode.Parse(value);
        }
        catch (JsonException)
        {
            // ignore, not JSON
        }

        if (node is null)
            return Deserialize(node: value);
        else
            return Deserialize(node);
    }

    /// <summary>Deserializes a JSON node to an inferred NBT tag.</summary>
    /// <param name="node">The JSON node.</param>
    /// <returns>The inferred NBT tag.</returns>
    /// <exception cref="JsonException">No tag can be deserialized from the node.</exception>
    public static NbtTag Deserialize(JsonNode node)
    {
        return JsonSerializer.Deserialize<NbtTag>(node, Options) ?? throw new JsonException("Nbt cannot be deserialized from JSON.");
    }
}
