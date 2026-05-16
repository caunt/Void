
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Api.Plugins.Dependencies;

public interface IDependencyService : IEventListener, IServiceProvider
{
    public TService CreateInstance<TService>(CancellationToken cancellationToken = default, params object[] parameters);
    public TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters);
    public object CreateInstance(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters);
    public bool TryGetPlayerContext(IEventListener listener, [MaybeNullWhen(false)] out IPlayerContext context);
    public IServiceProvider GetEntryPoint(IPlayer player, Assembly? preferredAssembly = null);
    public IServiceProvider GetEntryPoint(Assembly? preferredAssembly = null);
    public void ActivatePlayerScope(IPlayerContext context);
    public void DisposePlayerScope(IPlayerContext context);
    public TService? GetService<TService>();
    public void Register(Action<ServiceCollection> configure, bool activate = true);
}
