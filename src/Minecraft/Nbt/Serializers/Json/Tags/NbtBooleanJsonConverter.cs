using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtBooleanJsonConverter : JsonConverter<NbtBoolean>
{
    public override NbtBoolean Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetBoolean());

    public override void Write(Utf8JsonWriter writer, NbtBoolean value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, (NbtByte)value, options);
}
