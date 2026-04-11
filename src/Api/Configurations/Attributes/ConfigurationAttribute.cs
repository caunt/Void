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
    public string Name { get; init; } = name;
    public string? InlineComment { get; init; }
    public string? PrecedingComment { get; init; }
}
