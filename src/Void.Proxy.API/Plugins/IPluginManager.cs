using System.Reflection;

namespace Void.Proxy.API.Plugins;

public interface IPluginManager
{
    public IPlugin[] RegisterPlugins(Assembly assembly);
    public void UnregisterPlugins(IPlugin[] plugins);
}
