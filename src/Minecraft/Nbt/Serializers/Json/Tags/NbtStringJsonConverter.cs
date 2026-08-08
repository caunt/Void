using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

/// <summary>Converts between NBT string tags and non-null JSON strings.</summary>
public class NbtStringJsonConverter : JsonConverter<NbtString>
{
    /// <inheritdoc/>
    public override NbtString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetString() ?? throw new JsonException($"{nameof(NbtString)} value cannot be null."));

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtString tag, JsonSerializerOptions options) => writer.WriteStringValue(tag.Value);
}
