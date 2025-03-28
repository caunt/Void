using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json;

public class NbtTagJsonConverter : JsonConverter<NbtTag>
{
    public override NbtTag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
    {
        JsonTokenType.Number => DeserializeNumber(ref reader),
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

    private static NbtTag DeserializeNumber(ref Utf8JsonReader reader)
    {
        if (reader.TryGetByte(out var byteValue))
            return new NbtByte(byteValue);

        if (reader.TryGetInt16(out var shortValue))
            return new NbtShort(shortValue);

        if (reader.TryGetInt32(out var intValue))
            return new NbtInt(intValue);

        if (reader.TryGetInt64(out var longValue))
            return new NbtLong(longValue);

        if (reader.TryGetSingle(out var floatValue) && floatValue is not float.PositiveInfinity and not float.NegativeInfinity)
            return new NbtFloat(floatValue);

        if (reader.TryGetDouble(out var doubleValue))
            return new NbtDouble(doubleValue);

        throw new JsonException($"\"{Encoding.UTF8.GetString(reader.ValueSpan)}\" is not a valid NBT number.");
    }
}
