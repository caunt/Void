using System.Reflection;
using Void.Common.Plugins;

namespace Void.Proxy.Api.Plugins.Services;

public interface IPluginService
{
    public IReadOnlyList<IPlugin> All { get; }

    public ValueTask LoadEmbeddedPluginsAsync(CancellationToken cancellationToken = default);
    public ValueTask LoadPluginsAsync(string path = "plugins", CancellationToken cancellationToken = default);
    public ValueTask LoadPluginsAsync(string assemblyName, Stream assemblyStream, CancellationToken cancellationToken = default);

    public ValueTask UnloadPluginsAsync(CancellationToken cancellationToken = default);
    public ValueTask UnloadPluginAsync(string assemblyName, CancellationToken cancellationToken = default);

    public IPlugin[] GetPlugins(string assemblyName, Assembly assembly);
    public void RegisterPlugin(IPlugin plugin);
    public void UnregisterPlugin(IPlugin plugin);
}
