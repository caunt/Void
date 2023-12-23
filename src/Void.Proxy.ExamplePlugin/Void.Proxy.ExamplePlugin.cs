using Void.Proxy.Plugins.API;
using Void.Proxy.Plugins.API.Events;

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
