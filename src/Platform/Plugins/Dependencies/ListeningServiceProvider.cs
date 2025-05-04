using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Plugins.Dependencies;

public class ListeningServiceProvider(IServiceProvider source, CancellationToken cancellationToken) : IServiceProvider
{
    private readonly IEventService _events = source.GetRequiredService<IEventService>();

    public IServiceProvider Source => source;
    public CancellationToken CancellationToken => cancellationToken;

    public static IServiceProvider Wrap(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return new ListeningServiceProvider(serviceProvider, cancellationToken);
    }

    public object? GetService(Type serviceType)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var instance = source.GetService(serviceType);

        if (instance is IEventListener listener)
            _events.RegisterListeners(cancellationToken, listener);

        return instance;
    }
}
