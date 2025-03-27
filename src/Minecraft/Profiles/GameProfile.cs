using System.Text.Json.Serialization;

namespace Void.Minecraft.Profiles;

public record GameProfile(
    [property: JsonConverter(typeof(Uuid.JsonConverter))]
    Uuid Id,
    [property: JsonPropertyName("name")] string Username,
    Property[] Properties);