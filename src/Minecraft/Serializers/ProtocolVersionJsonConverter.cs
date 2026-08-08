using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Network;

namespace Void.Minecraft.Serializers;

/// <summary>
/// Converts protocol versions to and from their numeric Minecraft protocol identifiers in JSON.
/// </summary>
public sealed class ProtocolVersionJsonConverter : JsonConverter<ProtocolVersion>
{
    /// <summary>
    /// Reads a protocol version from the current JSON number token.
    /// </summary>
    /// <param name="reader">The JSON reader positioned at a numeric protocol identifier.</param>
    /// <param name="typeToConvert">The target type requested by the serializer.</param>
    /// <param name="options">The active serializer options.</param>
    /// <returns>The known protocol version matching the identifier, or a dynamically created unknown version.</returns>
    public override ProtocolVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ProtocolVersion.From(reader.GetInt32());
    }

    /// <summary>
    /// Writes the numeric identifier of a protocol version as a JSON number.
    /// </summary>
    /// <param name="writer">The destination JSON writer.</param>
    /// <param name="value">The protocol version to write.</param>
    /// <param name="options">The active serializer options.</param>
    public override void Write(Utf8JsonWriter writer, ProtocolVersion value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
