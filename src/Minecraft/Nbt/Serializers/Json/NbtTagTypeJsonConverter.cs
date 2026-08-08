using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Minecraft.Nbt.Serializers.Json;

/// <summary>Writes NBT tag-type identifiers as their numeric byte values.</summary>
/// <remarks>Deserialization is not supported.</remarks>
public class NbtTagTypeJsonConverter : JsonConverter<NbtTagType>
{
    /// <inheritdoc/>
    public override NbtTagType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtTagType tag, JsonSerializerOptions options) => writer.WriteNumberValue((byte)tag);
}
