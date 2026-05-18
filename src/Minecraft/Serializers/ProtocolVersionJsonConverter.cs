using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Network;

namespace Void.Minecraft.Serializers;

public sealed class ProtocolVersionJsonConverter : JsonConverter<ProtocolVersion>
{
    public override ProtocolVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ProtocolVersion.From(reader.GetInt32());
    }

    public override void Write(Utf8JsonWriter writer, ProtocolVersion value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
