
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Api.Plugins.Dependencies;

public interface IDependencyService : IEventListener, IServiceProvider
{
    public TService CreateInstance<TService>(CancellationToken cancellationToken = default);
    public TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default);
    public object CreateInstance(Type serviceType, CancellationToken cancellationToken = default);
    public bool TryGetScopedPlayerContext(object instance, [MaybeNullWhen(false)] out IPlayerContext context);
    public IServiceProvider CreatePlayerComposite(IPlayer player);
    public void ActivatePlayerContext(IPlayerContext context);
    public void DisposePlayerContext(IPlayerContext context);
    public TService? GetService<TService>();
    public void Register(Action<ServiceCollection> configure, bool activate = true);
}
