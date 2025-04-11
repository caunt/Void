using Microsoft.Extensions.Hosting;
using Void.Common.Plugins;

namespace Void.Proxy.Api.Configurations;

public interface IConfigurationService : IHostedService
{
    public TConfiguration Get<TConfiguration>(IPlugin plugin) where TConfiguration : IConfiguration;
}
