using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

/// <summary>Writes NBT signed 64-bit integer tags as JSON numbers.</summary>
/// <remarks>Deserialization is not supported.</remarks>
public class NbtLongJsonConverter : JsonConverter<NbtLong>
{
    /// <inheritdoc/>
    public override NbtLong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtLong tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
