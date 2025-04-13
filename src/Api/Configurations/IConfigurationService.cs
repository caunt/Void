using Microsoft.Extensions.Hosting;

namespace Void.Proxy.Api.Configurations;

public interface IConfigurationService : IHostedService
{
    /// <summary>
    /// Asynchronously retrieves a configuration of a specified type.
    /// </summary>
    /// <typeparam name="TConfiguration">Represents the type of configuration being retrieved, which must not be null.</typeparam>
    /// <param name="cancellationToken">Used to signal the cancellation of the asynchronous operation.</param>
    /// <returns>Returns a value task that resolves to the requested configuration.</returns>
    public ValueTask<TConfiguration> GetAsync<TConfiguration>(CancellationToken cancellationToken = default) where TConfiguration : notnull;

    /// <summary>
    /// Asynchronously retrieves a configuration value based on a specified key.
    /// </summary>
    /// <typeparam name="TConfiguration">Represents the type of the configuration value being retrieved.</typeparam>
    /// <param name="key">Specifies the identifier for the configuration value to be fetched.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns a task that resolves to the requested configuration value.</returns>
    public ValueTask<TConfiguration> GetAsync<TConfiguration>(string key, CancellationToken cancellationToken = default) where TConfiguration : notnull;
}
