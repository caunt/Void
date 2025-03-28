using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Tags;

public class NbtIntJsonConverter : JsonConverter<NbtInt>
{
    public override NbtInt Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new NbtInt(reader.GetInt32());

    public override void Write(Utf8JsonWriter writer, NbtInt tag, JsonSerializerOptions options) => writer.WriteNumberValue(tag.Value);
}
