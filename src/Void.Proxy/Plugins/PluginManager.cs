using Nito.Disposables.Internals;
using System.Diagnostics;
using System.Reflection;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Reflection;

namespace Void.Proxy.Plugins;

public class PluginManager : IPluginManager
{
    private readonly List<IPlugin> _plugins = [];
    private readonly List<WeakPluginReference> _references = [];
    private readonly TimeSpan _unloadTimeout = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _gcRate = TimeSpan.FromMilliseconds(500);

    public async Task LoadAsync(string path = "plugins", CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var pluginPaths = Directory.GetFiles(path, "*.dll");

        Proxy.Logger.Information($"Loading {pluginPaths.Length} plugins");

        var pluginInterface = typeof(IPlugin);

        foreach (var pluginPath in pluginPaths)
        {
            var context = new PluginLoadContext(pluginPath);

            Proxy.Logger.Information($"Loading {context.Name} plugin");

            var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginPath)));
            var plugins = assembly
                .GetTypes()
                .Where(pluginInterface.IsAssignableFrom)
                .Select(Activator.CreateInstance)
                .Cast<IPlugin?>()
                .WhereNotNull()
                .ToArray();

            if (plugins.Length == 0)
                throw new InvalidDataException($"Couldn't load {context.Name} plugin, not found any IPlugin implementations");

            _plugins.AddRange(plugins);
            _references.Add(new(context, plugins));

            foreach (var plugin in plugins)
                await plugin.ExecuteAsync(cancellationToken);

            Proxy.Logger.Debug($"Loaded {plugins.Length} plugins from {context.Name}");
        }
    }

    public async Task UnloadAsync(CancellationToken cancellationToken = default)
    {
        foreach (var reference in _references)
        {
            if (!reference.IsAlive)
                throw new Exception("Plugin context already unloaded");

            var name = reference.Context.Name;

            Array.ForEach(reference.Plugins, RemovePlugin);
            reference.Context.Unload();

            Proxy.Logger.Information($"Unloading {name} plugin");

            var collectionTime = Stopwatch.GetTimestamp();

            do
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!reference.IsAlive)
                    break;

                await Task.Delay(_gcRate, cancellationToken);
            }
            while (Stopwatch.GetElapsedTime(collectionTime) < _unloadTimeout);

            if (reference.IsAlive)
                throw new Exception($"Plugin {name} refuses to unload");
            else
                Proxy.Logger.Information($"Plugin {name} unloaded successfully");
        }

        _references.RemoveAll(reference => !reference.IsAlive);
    }

    internal void RemovePlugin(IPlugin plugin) => _plugins.Remove(plugin);
}
