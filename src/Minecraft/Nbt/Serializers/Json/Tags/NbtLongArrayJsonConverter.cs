using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtLongArrayJsonConverter : JsonConverter<NbtLongArray>
{
    public override NbtLongArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, NbtLongArray tag, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var value in tag.Data)
            writer.WriteNumberValue(value);

        writer.WriteEndArray();
    }
}
