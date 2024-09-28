using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins;

public class PluginServiceProvider(IServiceProvider main, IServiceProvider child) : IPluginServiceProvider
{
    public object? GetService(Type serviceType)
    {
        return main.GetService(serviceType) ?? child.GetService(serviceType);
    }
}
