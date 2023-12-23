using Void.Proxy.API.Events;
using Void.Proxy.API.Plugins;

namespace VoidTestPlugin;

public class ExamplePlugin : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("i am loaded");
        return Task.CompletedTask;
    }

    [Subscribe<IEvent>]
    public void Test()
    {

    }
}
