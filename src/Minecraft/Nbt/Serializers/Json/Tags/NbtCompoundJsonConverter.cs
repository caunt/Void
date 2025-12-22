using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtCompoundJsonConverter : JsonConverter<NbtCompound>
{
    public override NbtCompound Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var compound = new NbtCompound();

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
                break;

            if (reader.TokenType is not JsonTokenType.PropertyName)
                throw new JsonException($"Expected {nameof(JsonTokenType.PropertyName)} token.");

            var propertyName = reader.GetString() ?? throw new JsonException("Expected property name, but got null.");

            if (!reader.Read())
                throw new JsonException("Unexpected end when reading property value.");

            var tag = JsonSerializer.Deserialize<NbtTag>(ref reader, options) ?? throw new JsonException("Expected tag, but got null.");
            compound[propertyName] = tag;
        }

        return compound;
    }

    public override void Write(Utf8JsonWriter writer, NbtCompound tag, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var (propertyName, propertyTag) in tag.Fields)
        {
            writer.WritePropertyName(propertyName);
            JsonSerializer.Serialize(writer, propertyTag, options);
        }

        writer.WriteEndObject();
    }
}
