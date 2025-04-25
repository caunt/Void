using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Plugins.Dependencies;

public class ListeningServiceProvider(IServiceProvider source) : IServiceProvider
{
    private readonly IEventService _events = source.GetRequiredService<IEventService>();

    public IServiceProvider Source => source;

    public static IServiceProvider Wrap(IServiceProvider serviceProvider)
    {
        return new ListeningServiceProvider(serviceProvider);
    }

    public object? GetService(Type serviceType)
    {
        var instance = source.GetService(serviceType);

        if (instance is IEventListener listener)
            _events.RegisterListeners(listener);

        return instance;
    }
}
