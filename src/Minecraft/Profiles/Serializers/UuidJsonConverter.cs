using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Minecraft.Profiles.Serializers;

/// <summary>
/// Converts <see cref="Uuid" /> values to and from their standard JSON string representation.
/// </summary>
public class UuidJsonConverter : JsonConverter<Uuid>
{
    /// <summary>
    /// Reads a UUID from the current JSON string token.
    /// </summary>
    /// <param name="reader">The JSON reader positioned at the UUID string.</param>
    /// <param name="typeToConvert">The target type requested by the serializer.</param>
    /// <param name="options">The active serializer options.</param>
    /// <returns>The parsed UUID.</returns>
    /// <exception cref="JsonException">The JSON token contains a null string.</exception>
    /// <exception cref="FormatException">The string is not a recognized UUID representation.</exception>
    public override Uuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Uuid.Parse(reader.GetString() ?? throw new JsonException($"{nameof(Uuid)} value cannot be null."));
    }

    /// <summary>
    /// Writes a UUID as a JSON string value.
    /// </summary>
    /// <param name="writer">The destination JSON writer.</param>
    /// <param name="value">The UUID to write.</param>
    /// <param name="options">The active serializer options.</param>
    public override void Write(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    /// <summary>
    /// Reads a UUID encoded as a JSON property name.
    /// </summary>
    /// <param name="reader">The JSON reader positioned at the property name.</param>
    /// <param name="typeToConvert">The target type requested by the serializer.</param>
    /// <param name="options">The active serializer options.</param>
    /// <returns>The parsed UUID.</returns>
    /// <exception cref="JsonException">The property name is null.</exception>
    /// <exception cref="FormatException">The property name is not a recognized UUID representation.</exception>
    public override Uuid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = reader.GetString() ??
            throw new JsonException($"{nameof(Uuid)} property name cannot be null.");

        return Uuid.Parse(name);
    }

    /// <summary>
    /// Writes a UUID as a JSON property name.
    /// </summary>
    /// <param name="writer">The destination JSON writer.</param>
    /// <param name="value">The UUID to write.</param>
    /// <param name="options">The active serializer options.</param>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString());
    }
}
