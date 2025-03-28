using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json;

public class NbtTagJsonConverter : JsonConverter<NbtTag>
{
    public override NbtTag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
    {
        JsonTokenType.Number => NbtTagNumberAdapter.DeserializeNumber(ref reader),
        JsonTokenType.String => JsonSerializer.Deserialize<NbtString>(ref reader, options),
        JsonTokenType.StartObject => JsonSerializer.Deserialize<NbtCompound>(ref reader, options),
        JsonTokenType.StartArray => JsonSerializer.Deserialize<NbtList>(ref reader, options),
        JsonTokenType.True or JsonTokenType.False => JsonSerializer.Deserialize<NbtBoolean>(ref reader, options),
        JsonTokenType.Null => new NbtCompound(),
        var value => throw new NotSupportedException(value.ToString())
    } ?? throw new JsonException("One of json converters returned null in read method.");

    public override void Write(Utf8JsonWriter writer, NbtTag tag, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)tag, options);
    }
}
