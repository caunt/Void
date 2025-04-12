namespace Void.Proxy.Api.Configurations.Serializer;

public interface IConfigurationSerializer
{
    public string Serialize<TConfiguration>() where TConfiguration : notnull;
    public string Serialize<TConfiguration>(TConfiguration configuration) where TConfiguration : notnull;
    public TConfiguration Deserialize<TConfiguration>(string source) where TConfiguration : notnull;
}
