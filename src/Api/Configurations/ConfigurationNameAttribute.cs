namespace Void.Proxy.Api.Configurations;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class ConfigurationNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
