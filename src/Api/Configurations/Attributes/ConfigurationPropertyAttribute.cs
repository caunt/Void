namespace Void.Proxy.Api.Configurations.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConfigurationPropertyAttribute(string? name = null) : Attribute
{
    public string? Name { get; init; } = name;
    public string? InlineComment { get; init; }
    public string? PrecedingComment { get; init; }
}
