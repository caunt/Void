namespace Void.Proxy.Api.Configurations;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
public class ConfigurationNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
