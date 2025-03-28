using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtDoubleJsonConverter : JsonConverter<NbtDouble>
{
    public override NbtDouble Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, NbtDouble tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
