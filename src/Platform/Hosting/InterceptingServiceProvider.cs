using Void.Common.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Hosting;

public class InterceptingServiceProvider(IServiceProvider provider) : IServiceProvider, ISupportRequiredService, IDisposable, IAsyncDisposable
{
    private readonly IEventService _events = provider.GetRequiredService<IEventService>();

    public object? GetService(Type serviceType)
    {
        var instance = provider.GetService(serviceType);

        if (instance is IEventListener listener)
            _events.RegisterListeners(listener);

        return instance;
    }

    public object GetRequiredService(Type serviceType)
    {
        var instance = ((ISupportRequiredService)provider).GetRequiredService(serviceType);

        if (instance is IEventListener listener)
            _events.RegisterListeners(listener);

        return instance;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (provider is not IDisposable disposable)
            return;

        disposable.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (provider is IAsyncDisposable disposable)
            await disposable.DisposeAsync();
        else
            Dispose();
    }
}
