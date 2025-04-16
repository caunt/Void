
using Void.Common.Events;

namespace Void.Proxy.Api.Plugins;

public interface IDependencyService : IEventListener
{
    public TService CreateInstance<TService>();
    public TService CreateInstance<TService>(Type serviceType);
    public object CreateInstance(Type serviceType);
    public TService GetRequiredService<TService>() where TService : notnull;
}
