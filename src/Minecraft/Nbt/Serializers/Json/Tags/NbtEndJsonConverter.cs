using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

/// <summary>Writes an NBT end tag by closing the current JSON object.</summary>
/// <remarks>Deserialization is not supported.</remarks>
public class NbtEndJsonConverter : JsonConverter<NbtEnd>
{
    /// <inheritdoc/>
    public override NbtEnd Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtEnd tag, JsonSerializerOptions options) => writer.WriteEndObject();
}
