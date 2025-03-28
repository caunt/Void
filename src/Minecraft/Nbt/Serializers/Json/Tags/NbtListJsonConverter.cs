using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtListJsonConverter : JsonConverter<NbtList>
{
    public override NbtList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = new List<NbtTag>();

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndArray)
                break;

            var tag = JsonSerializer.Deserialize<NbtTag>(ref reader, options) ?? throw new JsonException("Expected tag, but got null.");
            values.Add(tag);
        }

        NbtTagNumberAdapter.AlignNumbers(values);
        return new NbtList(values, NbtTagNumberAdapter.GetTagsType(values));
    }

    public override void Write(Utf8JsonWriter writer, NbtList tag, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var value in tag.Data)
            JsonSerializer.Serialize(writer, value, options);

        writer.WriteEndArray();
    }
}
