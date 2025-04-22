
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Plugins.Dependencies;

public interface IDependencyService : IEventListener, IServiceProvider
{
    public TService CreateInstance<TService>();
    public TService CreateInstance<TService>(Type serviceType);
    public object CreateInstance(Type serviceType);
    public TService? GetService<TService>();
    public void Register(Action<ServiceCollection> configure, bool activate = true);
    public IServiceProvider CreatePlayerScope(IPlayer player);
}
