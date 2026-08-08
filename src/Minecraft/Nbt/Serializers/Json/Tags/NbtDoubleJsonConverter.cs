using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

/// <summary>
/// Converts an <see cref="NbtDouble"/> tag to a JSON number containing its double-precision value.
/// </summary>
/// <remarks>
/// This converter supports serialization only. Deserialization through <see cref="Read"/> throws a <see cref="NotSupportedException"/>.
/// </remarks>
public class NbtDoubleJsonConverter : JsonConverter<NbtDouble>
{
    /// <inheritdoc/>
    public override NbtDouble Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, NbtDouble tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
