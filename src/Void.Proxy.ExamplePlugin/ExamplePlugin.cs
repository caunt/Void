using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.ExamplePlugin;

public class ExamplePlugin : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [Subscribe]
    public void Test(SearchProtocolCodec @event)
    {
        Console.WriteLine("received event");
    }
}
