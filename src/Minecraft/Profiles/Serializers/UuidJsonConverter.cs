using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Minecraft.Profiles.Serializers;

public class UuidJsonConverter : JsonConverter<Uuid>
{
    public override Uuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Uuid.Parse(reader.GetString() ?? throw new JsonException($"{nameof(Uuid)} value cannot be null."));
    }

    public override void Write(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override Uuid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = reader.GetString() ??
            throw new JsonException($"{nameof(Uuid)} property name cannot be null.");

        return Uuid.Parse(name);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString());
    }
}
