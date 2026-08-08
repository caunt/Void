using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

/// <summary>Writes NBT single-precision tags as JSON numbers.</summary>
/// <remarks>Deserialization is not supported.</remarks>
public class NbtFloatJsonConverter : JsonConverter<NbtFloat>
{
    /// <inheritdoc/>
    public override NbtFloat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtFloat tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
