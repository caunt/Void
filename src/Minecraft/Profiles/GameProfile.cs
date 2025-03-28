using System.Text.Json.Serialization;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Profiles;

public record GameProfile(
    [property: JsonConverter(typeof(UuidJsonConverter))]
    Uuid Id,
    [property: JsonPropertyName("name")] string Username,
    Property[] Properties);