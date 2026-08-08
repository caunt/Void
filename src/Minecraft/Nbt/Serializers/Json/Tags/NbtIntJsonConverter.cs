using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

/// <summary>Converts between NBT signed 32-bit integer tags and JSON numbers.</summary>
public class NbtIntJsonConverter : JsonConverter<NbtInt>
{
    /// <inheritdoc/>
    public override NbtInt Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetInt32());

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtInt tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
