namespace Void.Proxy.Api.Configurations.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
/// <summary>
/// Provides TOML mapping metadata for a configuration field or property.
/// </summary>
/// <param name="name">
/// Optional serialized key name used by the TOML serializer instead of the member name.
/// </param>
/// <remarks>
/// During TOML mapping, null or whitespace values are ignored.
/// </remarks>
public class ConfigurationPropertyAttribute(string? name = null) : Attribute
{
    /// <summary>
    /// Gets the serialized key name override.
    /// </summary>
    /// <value>
    /// The TOML key name when provided; otherwise <see langword="null"/>.
    /// </value>
    public string? Name { get; init; } = name;

    /// <summary>
    /// Gets an inline comment to attach to the serialized TOML member.
    /// </summary>
    /// <value>
    /// The inline comment text, or <see langword="null"/> when not set.
    /// </value>
    public string? InlineComment { get; init; }

    /// <summary>
    /// Gets a preceding comment to attach above the serialized TOML member.
    /// </summary>
    /// <value>
    /// The preceding comment text, or <see langword="null"/> when not set.
    /// </value>
    public string? PrecedingComment { get; init; }
}
