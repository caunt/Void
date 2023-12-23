using Void.Proxy.Plugins.API.Events;

namespace Void.Proxy.Plugins.API;

public interface IPlugin : IEventListener
{
    public string Name { get; }

    public Task ExecuteAsync(CancellationToken cancellationToken);
}