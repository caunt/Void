using System.Text.Json.Serialization;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Profiles;

public record GameProfile(
    [property: JsonPropertyName("name")] string Username,
    [property: JsonConverter(typeof(UuidJsonConverter))] Uuid Id = default,
    Property[]? Properties = null);
