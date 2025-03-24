using System.Text.Json.Serialization;
using Void.Minecraft;

namespace Void.Proxy.Api.Mojang.Profiles;

public record GameProfile(
    [property: JsonConverter(typeof(Uuid.JsonConverter))]
    Uuid Id,
    [property: JsonPropertyName("name")] string Username,
    Property[] Properties);