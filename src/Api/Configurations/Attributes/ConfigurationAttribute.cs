namespace Void.Proxy.Api.Configurations.Attributes;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
internal class RootConfigurationAttribute(string name) : ConfigurationAttribute(name);


[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class ConfigurationAttribute(string name) : Attribute
{
    public string Name { get; init; } = name;
    public string? InlineComment { get; init; }
    public string? PrecedingComment { get; init; }
}
