namespace Void.Proxy.Api.Configurations.Attributes;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
internal class RootConfigurationAttribute(string name) : ConfigurationAttribute(name);


/// <summary>
/// Declares TOML metadata for a configuration root type.
/// </summary>
/// <param name="name">
/// Optional configuration name override used by the configuration file naming pipeline.
/// When <paramref name="name"/> is <see langword="null"/>, empty, or whitespace, consumers fall back to the CLR type name.
/// </param>
/// <remarks>
/// <para>
/// This attribute is read when resolving configuration file names and when projecting type-level TOML comments.
/// </para>
/// <para>
/// The <see cref="Name"/> value is treated as a logical alias and does not rename CLR members.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [Configuration("network")]
/// public class NetworkConfiguration
/// {
/// }
/// </code>
/// </example>
/// <seealso cref="RootConfigurationAttribute"/>
/// <seealso cref="ConfigurationPropertyAttribute"/>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class ConfigurationAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the logical configuration name override declared for the decorated configuration type.
    /// </summary>
    /// <value>
    /// The configured alias used by configuration file naming. Consumers treat <see langword="null"/>, empty, and whitespace-only values as unspecified and fall back to the CLR type name.
    /// </value>
    /// <remarks>
    /// <para>
    /// This value affects naming in configuration storage and does not rename CLR members or change type identity.
    /// </para>
    /// <para>
    /// The property is <see langword="init"/>-only, so it can be assigned during attribute construction and object initialization.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [Configuration("network")]
    /// public sealed class NetworkConfiguration
    /// {
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ConfigurationAttribute"/>
    /// <seealso cref="RootConfigurationAttribute"/>
    public string Name { get; init; } = name;
    public string? InlineComment { get; init; }
    public string? PrecedingComment { get; init; }
}
