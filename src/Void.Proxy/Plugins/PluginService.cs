using System.Diagnostics;
using System.Reflection;
using Nito.Disposables.Internals;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Plugins;
using Void.Proxy.Reflection;

namespace Void.Proxy.Plugins;

public class PluginService(
    ILogger<PluginService> logger,
    IEventService events,
    IServiceProvider services) : IPluginService
{
    private readonly List<IPlugin> _plugins = [];
    private readonly List<WeakPluginReference> _references = [];
    private readonly TimeSpan _gcRate = TimeSpan.FromMilliseconds(500);
    private readonly TimeSpan _unloadTimeout = TimeSpan.FromSeconds(10);

    public async ValueTask LoadAsync(string path = "plugins", CancellationToken cancellationToken = default)
    {
#if DEBUG
        var directory = new DirectoryInfo(Environment.CurrentDirectory);

        while (directory != null && directory.Name != "src")
            directory = directory.Parent;

        var pluginsDirectory = Path.Combine(Environment.CurrentDirectory, "plugins");

        foreach (var pluginDirectory in directory!.GetDirectories("Void.Proxy.Plugins.*"))
        {
            if (pluginDirectory.Name.EndsWith("API"))
                continue;

            foreach (var file in Directory.GetFiles(Path.Combine(pluginDirectory.FullName, "bin", "Debug", "net9.0"), "*.dll"))
                File.Copy(file, Path.Combine(pluginsDirectory, Path.GetFileName(file)), true);
        }
#endif

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var pluginPaths = Directory.GetFiles(path, "*.dll");

        logger.LogInformation("Loading {Count} plugins", pluginPaths.Length);

        foreach (var pluginPath in pluginPaths)
        {
            var context = new PluginLoadContext(pluginPath);

            logger.LogInformation("Loading {PluginName} plugin", context.Name);

            var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginPath)));
            var plugins = RegisterPlugins(assembly);

            if (plugins.Length == 0)
            {
                logger.LogWarning("Plugin {PluginName} has no IPlugin implementations", context.Name);
                continue;
            }

            var listeners = assembly.GetTypes()
                .Where(typeof(IEventListener).IsAssignableFrom)
                .Select(CreateListenerInstance)
                .Cast<IEventListener?>()
                .WhereNotNull()
                .ToArray();

            if (listeners.Length == 0)
                logger.LogWarning("Plugin {PluginName} has no event listeners", context.Name);

            events.RegisterListeners(listeners);
            _references.Add(new WeakPluginReference(context, plugins, listeners));

            foreach (var plugin in plugins)
                await events.ThrowAsync(new PluginLoadEvent { Plugin = plugin }, cancellationToken);

            logger.LogDebug("Loaded {Count} plugins from {PluginName}", plugins.Length, context.Name);
        }
    }

    public async ValueTask UnloadAsync(CancellationToken cancellationToken = default)
    {
        foreach (var reference in _references)
        {
            if (!reference.IsAlive)
                throw new Exception("Plugin context already unloaded");

            foreach (var plugin in reference.Plugins)
                await events.ThrowAsync(new PluginUnloadEvent { Plugin = plugin }, cancellationToken);

            var name = reference.Context.Name;

            UnregisterPlugins(reference.Plugins);
            events.UnregisterListeners(reference.Listeners);

            reference.Context.Unload();

            logger.LogInformation($"Unloading {name} plugin");

            var collectionTime = Stopwatch.GetTimestamp();

            do
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!reference.IsAlive)
                    break;

                await Task.Delay(_gcRate, cancellationToken);
            } while (Stopwatch.GetElapsedTime(collectionTime) < _unloadTimeout);

            if (reference.IsAlive)
                throw new Exception($"Plugin {name} refuses to unload");

            logger.LogDebug($"Plugin {name} unloaded successfully");
        }

        _references.RemoveAll(reference => !reference.IsAlive);
    }

    public IPlugin[] RegisterPlugins(Assembly assembly)
    {
        var pluginInterface = typeof(IPlugin);
        var plugins = assembly.GetTypes()
            .Where(pluginInterface.IsAssignableFrom)
            .Select(CreatePluginInstance)
            .Cast<IPlugin?>()
            .WhereNotNull()
            .ToArray();

        _plugins.AddRange(plugins);
        return plugins;
    }

    public void UnregisterPlugins(IPlugin[] plugins)
    {
        foreach (var plugin in plugins)
            _plugins.Remove(plugin);
    }

    internal object? GetExistingInstance(Type type)
    {
        return _plugins.FirstOrDefault(plugin => plugin.GetType()
            .IsAssignableFrom(type));
    }

    private object CreatePluginInstance(Type type)
    {
        return ActivatorUtilities.CreateInstance(services, type);
    }

    private object? CreateListenerInstance(Type type)
    {
        if (GetExistingInstance(type) is { } instance)
            return instance;

        return Activator.CreateInstance(type);
    }
}