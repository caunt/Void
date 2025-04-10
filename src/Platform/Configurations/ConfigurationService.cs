using NuGet.Protocol.Plugins;
using Void.Proxy.Api.Configurations;
using IConfiguration = Void.Proxy.Api.Configurations.IConfiguration;

namespace Void.Proxy.Configurations;

public class ConfigurationService : IConfigurationService
{
    public TConfiguration Get<TConfiguration>(IPlugin plugin) where TConfiguration : IConfiguration
    {
        return default!;
    }
}
