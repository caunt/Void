namespace Void.Proxy.Api.Configurations.Serializer;

public interface IConfigurationSerializer
{
    public string Serialize<TConfiguration>() where TConfiguration : notnull;
    public string Serialize(Type configurationType);
    public string Serialize(object configuration);
    public string Serialize<TConfiguration>(TConfiguration? configuration) where TConfiguration : notnull;
    public string Serialize(object? configuration, Type configurationType);
    public TConfiguration Deserialize<TConfiguration>(string source) where TConfiguration : notnull;
    public object Deserialize(string source, Type configurationType);
}
