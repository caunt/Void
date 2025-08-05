using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Nbt.Serializers.Json.Tags;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Nbt.Serializers.Json;

public static class NbtJsonSerializer
{
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

    public static JsonNode Serialize(NbtTag tag)
    {
        return JsonSerializer.SerializeToNode(tag, Options) ?? throw new JsonException("Nbt cannot be serialized to JSON.");
    }

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

    public static NbtTag Deserialize(JsonNode node)
    {
        return JsonSerializer.Deserialize<NbtTag>(node, Options) ?? throw new JsonException("Nbt cannot be deserialized from JSON.");
    }
}
