using Microsoft.Extensions.Hosting;

namespace Void.Proxy.Api.Configurations;

public interface IConfigurationService : IHostedService
{
    public ValueTask<TConfiguration> GetAsync<TConfiguration>(CancellationToken cancellationToken = default) where TConfiguration : notnull;
}
