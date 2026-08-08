using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Api.Plugins.Dependencies;

/// <summary>
/// Manages plugin dependency registrations, activation, and player-scoped service providers.
/// </summary>
public interface IDependencyService : IEventListener, IServiceProvider
{
    /// <summary>
    /// Resolves or constructs an instance of <typeparamref name="TService" />.
    /// </summary>
    /// <typeparam name="TService">The service type to create.</typeparam>
    /// <param name="cancellationToken">A token associated with event-listener registration when the created instance is a listener.</param>
    /// <param name="parameters">Explicit constructor arguments used when the service is not already registered.</param>
    /// <returns>The resolved or constructed service instance.</returns>
    public TService CreateInstance<TService>(CancellationToken cancellationToken = default, params object[] parameters);

    /// <summary>
    /// Resolves or constructs a runtime service type and casts it to <typeparamref name="TService" />.
    /// </summary>
    /// <typeparam name="TService">The type expected by the caller.</typeparam>
    /// <param name="serviceType">The runtime service type to resolve or construct.</param>
    /// <param name="cancellationToken">A token associated with event-listener registration when the created instance is a listener.</param>
    /// <param name="parameters">Explicit constructor arguments used when the service is not already registered.</param>
    /// <returns>The resolved or constructed instance cast to <typeparamref name="TService" />.</returns>
    /// <exception cref="InvalidOperationException">The created instance cannot be cast to <typeparamref name="TService" />.</exception>
    public TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters);

    /// <summary>
    /// Resolves or constructs an instance of a runtime service type.
    /// </summary>
    /// <param name="serviceType">The runtime service type to resolve or construct.</param>
    /// <param name="cancellationToken">A token associated with event-listener registration when the created instance is a listener.</param>
    /// <param name="parameters">Explicit constructor arguments used when the service is not already registered.</param>
    /// <returns>The resolved or constructed service instance.</returns>
    public object CreateInstance(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters);

    /// <summary>
    /// Creates a composite provider that searches a player's scoped services before root services.
    /// </summary>
    /// <param name="player">The player whose scopes are included.</param>
    /// <param name="preferredAssembly">An optional plugin assembly whose registrations should receive resolution priority.</param>
    /// <returns>A composite service provider for player-scoped and root resolution.</returns>
    public IServiceProvider GetEntryPoint(IPlayer player, Assembly? preferredAssembly = null);

    /// <summary>
    /// Creates a composite provider for root service resolution.
    /// </summary>
    /// <param name="preferredAssembly">An optional plugin assembly whose registrations should receive resolution priority.</param>
    /// <returns>A composite root service provider.</returns>
    public IServiceProvider GetEntryPoint(Assembly? preferredAssembly = null);

    /// <summary>
    /// Determines whether a service instance belongs to a player's active scope.
    /// </summary>
    /// <param name="player">The player whose scope is inspected.</param>
    /// <param name="service">The service instance to locate by reference identity.</param>
    /// <returns><see langword="true" /> when the exact instance is present in the player's scope; otherwise, <see langword="false" />.</returns>
    public bool IsInPlayerScope(IPlayer player, object service);

    /// <summary>
    /// Attempts to determine the registered lifetime of a service type.
    /// </summary>
    /// <param name="serviceType">The service type to inspect.</param>
    /// <param name="reuse">When this method returns, the mapped service lifetime; unregistered services produce <see cref="ServiceLifetime.Transient" />.</param>
    /// <returns><see langword="true" /> when the service has a registration; otherwise, <see langword="false" />.</returns>
    public bool TryGetServiceReuse(Type serviceType, out ServiceLifetime reuse);

    /// <summary>
    /// Creates and eagerly activates registered scoped services for a player context.
    /// </summary>
    /// <param name="context">The player context whose scopes are activated.</param>
    public void ActivatePlayerScope(IPlayerContext context);

    /// <summary>
    /// Unregisters scoped event listeners and disposes all plugin scopes associated with a player context.
    /// </summary>
    /// <param name="context">The player context whose scopes are disposed.</param>
    public void DisposePlayerScope(IPlayerContext context);

    /// <summary>
    /// Resolves an optional service from the composite provider associated with its assembly.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <returns>The resolved service, or <see langword="null" /> when no registration can supply it.</returns>
    public TService? GetService<TService>();

    /// <summary>
    /// Adds service registrations for a plugin assembly and optionally activates non-transient services immediately.
    /// </summary>
    /// <param name="configure">A callback that adds descriptors to a temporary service collection.</param>
    /// <param name="activate"><see langword="true" /> to resolve singleton services and existing-player scoped services immediately; otherwise, <see langword="false" />.</param>
    public void Register(Action<ServiceCollection> configure, bool activate = true);
}
