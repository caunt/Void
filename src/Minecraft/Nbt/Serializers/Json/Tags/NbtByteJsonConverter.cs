using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

/// <summary>Writes NBT byte tags as JSON numbers.</summary>
/// <remarks>Deserialization is not supported.</remarks>
public class NbtByteJsonConverter : JsonConverter<NbtByte>
{
    /// <inheritdoc/>
    public override NbtByte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtByte tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
