using Microsoft.Extensions.Hosting;

namespace Void.Proxy.Api.Configurations;

/// <summary>
/// Provides managed configuration instances whose in-memory and persisted values remain synchronized.
/// </summary>
/// <remarks>
/// The service participates in the host lifecycle through <see cref="IHostedService" />. Configuration instances returned by this service are shared and managed by the service rather than detached snapshots.
/// </remarks>
public interface IConfigurationService : IHostedService
{
    /// <summary>
    /// Retrieves a configuration instance of type <typeparamref name="TConfiguration"/>.
    /// The returned instance is fully self-managed: any changes made to the instance are automatically saved to disk, 
    /// and any changes from disk are automatically loaded into the instance.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of configuration being retrieved. It must not be null.</typeparam>
    /// <param name="cancellationToken">A token used to signal the cancellation of the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask{TConfiguration}"/> that resolves to the requested configuration.</returns>
    public ValueTask<TConfiguration> GetAsync<TConfiguration>(CancellationToken cancellationToken = default) where TConfiguration : notnull;

    /// <summary>
    /// Retrieves a configuration instance of type <typeparamref name="TConfiguration"/>.
    /// The returned instance is fully self-managed: any changes made to the instance are automatically saved to disk, 
    /// and any changes from disk are automatically loaded into the instance.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of configuration being retrieved. It must not be null.</typeparam>
    /// <param name="key">Specifies the identifier for the configuration value to be fetched.</param>
    /// <param name="cancellationToken">A token used to signal the cancellation of the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask{TConfiguration}"/> that resolves to the requested configuration.</returns>
    public ValueTask<TConfiguration> GetAsync<TConfiguration>(string key, CancellationToken cancellationToken = default) where TConfiguration : notnull;
}
