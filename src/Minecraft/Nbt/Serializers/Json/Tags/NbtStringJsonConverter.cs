using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtStringJsonConverter : JsonConverter<NbtString>
{
    public override NbtString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetString() ?? throw new JsonException($"{nameof(NbtString)} value cannot be null."));

    public override void Write(Utf8JsonWriter writer, NbtString tag, JsonSerializerOptions options) => writer.WriteStringValue(tag.Value);
}
