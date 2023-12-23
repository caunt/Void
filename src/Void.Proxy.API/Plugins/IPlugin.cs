using Void.Proxy.API.Events;

namespace Void.Proxy.API.Plugins;

public interface IPlugin : IEventListener
{
    public string Name { get; }

    public Task ExecuteAsync(CancellationToken cancellationToken);
}