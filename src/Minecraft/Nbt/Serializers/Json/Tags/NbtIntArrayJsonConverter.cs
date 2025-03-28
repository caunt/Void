using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtIntArrayJsonConverter : JsonConverter<NbtIntArray>
{
    public override NbtIntArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, NbtIntArray tag, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var value in tag.Data)
            writer.WriteNumberValue(value);

        writer.WriteEndArray();
    }
}
