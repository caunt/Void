using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Minecraft.Nbt.Serializers.Json;

public class NbtTagTypeJsonConverter : JsonConverter<NbtTagType>
{
    public override NbtTagType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, NbtTagType tag, JsonSerializerOptions options) => writer.WriteNumberValue((byte)tag);
}
