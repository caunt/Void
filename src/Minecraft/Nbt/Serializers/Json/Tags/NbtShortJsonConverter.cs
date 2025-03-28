using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtShortJsonConverter : JsonConverter<NbtShort>
{
    public override NbtShort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, NbtShort tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
