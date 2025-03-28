using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtFloatJsonConverter : JsonConverter<NbtFloat>
{
    public override NbtFloat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, NbtFloat tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
