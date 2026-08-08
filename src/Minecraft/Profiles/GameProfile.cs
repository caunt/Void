using System.Text.Json.Serialization;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Profiles;

/// <summary>
/// Represents a Minecraft player's authenticated or offline game profile.
/// </summary>
/// <param name="Username">The profile name reported for the player.</param>
/// <param name="Id">The profile UUID, or the zero UUID when no identifier is available.</param>
/// <param name="Properties">Optional profile properties such as signed textures. The supplied array is retained without copying.</param>
public record GameProfile(
    [property: JsonPropertyName("name")] string Username,
    [property: JsonConverter(typeof(UuidJsonConverter))] Uuid Id = default,
    Property[]? Properties = null);
