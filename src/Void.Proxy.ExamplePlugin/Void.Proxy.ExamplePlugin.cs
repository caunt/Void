using Void.Proxy.API.Events;
using Void.Proxy.API.Plugins;

namespace VoidTestPlugin;

public class VoidTestPlugin : IPlugin
{
    public string Name => nameof(VoidTestPlugin);

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
