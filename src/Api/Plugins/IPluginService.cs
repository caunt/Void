using System.Reflection;
using Void.Common.Plugins;

namespace Void.Proxy.Api.Plugins;

public interface IPluginService
{
    public IEnumerable<IPlugin> All { get; }
    public IEnumerable<string> Containers { get; }

    public ValueTask LoadEmbeddedPluginsAsync(CancellationToken cancellationToken = default);
    public ValueTask LoadPluginsAsync(string path = "plugins", CancellationToken cancellationToken = default);
    public ValueTask LoadPluginsAsync(IEnumerable<Type> plugins, CancellationToken cancellationToken = default);
    public ValueTask LoadPluginAsync(Type pluginType, CancellationToken cancellationToken = default);
    public IEnumerable<Type> LoadContainer(string name, Stream stream, bool ignoreEmpty = false);
    public IEnumerable<Type> GetPlugins(Assembly assembly);
    public ValueTask UnloadContainersAsync(CancellationToken cancellationToken = default);
    public ValueTask UnloadContainerAsync(string name, CancellationToken cancellationToken = default);
}
