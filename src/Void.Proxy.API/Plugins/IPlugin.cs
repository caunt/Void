using Serilog;
using Void.Proxy.API.Events;

namespace Void.Proxy.API.Plugins;

public interface IPlugin : IEventListener
{
    public ILogger Logger { get; init; }
    public string Name { get; }

    public Task ExecuteAsync(CancellationToken cancellationToken);
}