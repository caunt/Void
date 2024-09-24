using System.Reflection;

namespace Void.Proxy.API.Plugins;

public interface IPluginService
{
    public ValueTask LoadAsync(string path = "plugins", CancellationToken cancellationToken = default);
    public ValueTask UnloadAsync(CancellationToken cancellationToken = default);
    public IPlugin[] RegisterPlugins(string name, Assembly assembly);
    public void UnregisterPlugins(IPlugin[] plugins);
}