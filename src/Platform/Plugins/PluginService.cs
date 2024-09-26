using System.Diagnostics;
using System.Reflection;
using Nito.Disposables.Internals;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Plugins;
using Void.Proxy.Reflection;

namespace Void.Proxy.Plugins;

public class PluginService(ILogger<PluginService> logger, IEventService events, IServiceProvider services, IPluginDependencyService dependencies) : IPluginService
{
    private readonly TimeSpan _gcRate = TimeSpan.FromMilliseconds(500);
    private readonly List<IPlugin> _plugins = [];
    private readonly List<WeakPluginReference> _references = [];
    private readonly TimeSpan _unloadTimeout = TimeSpan.FromSeconds(10);

    public async ValueTask LoadEmbeddedPluginsAsync(CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var pluginsResources = assembly.GetManifestResourceNames().Where(name => name.Contains(nameof(Plugins)));

        foreach (var resourceName in pluginsResources)
        {
            if (assembly.GetManifestResourceStream(resourceName) is not { } stream)
            {
                logger.LogWarning("Embedded plugin {PluginName} couldn't be loaded", resourceName);
                continue;
            }

            await LoadPluginsAsync(resourceName, stream, cancellationToken);
            stream.Close();
        }
    }

    public async ValueTask LoadPluginsAsync(string path = "plugins", CancellationToken cancellationToken = default)
    {
        var pluginsDirectoryInfo = new DirectoryInfo(path);

        if (!pluginsDirectoryInfo.Exists)
            pluginsDirectoryInfo.Create();

        var pluginsFiles = pluginsDirectoryInfo.GetFiles("*.dll").Select(fileInfo => fileInfo.FullName).ToArray();
        logger.LogInformation("Loading {Count} plugins", pluginsFiles.Length);

        foreach (var pluginPath in pluginsFiles)
        {
            await using var stream = File.OpenRead(pluginPath);
            await LoadPluginsAsync(Path.GetFileName(pluginPath), stream, cancellationToken);
        }
    }

    public async ValueTask LoadPluginsAsync(string assemblyName, Stream assemblyStream, CancellationToken cancellationToken = default)
    {
        var context = new PluginLoadContext(services.GetRequiredService<ILogger<PluginLoadContext>>(), dependencies, assemblyName, assemblyStream);

        logger.LogInformation("Loading {PluginName} plugin", context.Name);

        var plugins = GetPlugins(assemblyName, context.PluginAssembly);

        foreach (var plugin in plugins)
            RegisterPlugin(plugin);

        if (plugins.Length == 0)
        {
            logger.LogWarning("Plugin {PluginName} has no IPlugin implementations", context.Name);
            return;
        }

        var listeners = context.PluginAssembly.GetTypes().Where(typeof(IEventListener).IsAssignableFrom).Select(CreateListenerInstance).Cast<IEventListener?>().WhereNotNull().ToArray();

        if (listeners.Length == 0)
            logger.LogWarning("Plugin {PluginName} has no event listeners", context.Name);

        events.RegisterListeners(listeners);
        _references.Add(new WeakPluginReference(context, plugins, listeners));

        foreach (var plugin in plugins)
            await events.ThrowAsync(new PluginLoadEvent { Plugin = plugin }, cancellationToken);

        logger.LogDebug("Loaded {Count} plugins from {PluginName}", plugins.Length, context.Name);
    }

    public async ValueTask UnloadPluginsAsync(CancellationToken cancellationToken = default)
    {
        for (var index = _references.Count - 1; index >= 0; index--)
            await UnloadPluginAsync(_references[index].Context.Name!, cancellationToken);
    }

    public async ValueTask UnloadPluginAsync(string assemblyName, CancellationToken cancellationToken = default)
    {
        foreach (var reference in _references.Where(reference => reference.Context.Name == assemblyName).ToArray())
        {
            if (!reference.IsAlive)
                throw new Exception("Plugin context already unloaded");

            var name = reference.Context.Name;

            foreach (var plugin in reference.Plugins)
            {
                await events.ThrowAsync(new PluginUnloadEvent { Plugin = plugin }, cancellationToken);
                UnregisterPlugin(plugin);
            }

            events.UnregisterListeners(reference.Listeners);

            reference.Context.Unload();

            logger.LogInformation("Unloading {PluginName} plugin", name);

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

            logger.LogDebug("Plugin {PluginName} unloaded successfully", name);
        }

        _references.RemoveAll(reference => !reference.IsAlive);
    }

    public IPlugin[] GetPlugins(string assemblyName, Assembly assembly)
    {
        var pluginInterface = typeof(IPlugin);

        try
        {
            var plugins = assembly.GetTypes().Where(pluginInterface.IsAssignableFrom).Select(CreatePluginInstance).Cast<IPlugin?>().WhereNotNull().ToArray();
            return plugins;
        }
        catch (ReflectionTypeLoadException exception)
        {
            logger.LogError("Assembly {AssemblyName} cannot be loaded:", assemblyName);

            var noStackTrace = exception.LoaderExceptions.WhereNotNull().Where(loaderException => string.IsNullOrWhiteSpace(loaderException.StackTrace)).ToArray();

            if (noStackTrace.Length == exception.LoaderExceptions.Length)
                logger.LogError("{Exceptions}", string.Join(", ", noStackTrace.Select(loaderException => loaderException.Message)));
            else
                logger.LogError(exception, "Multiple Exceptions:");
        }

        return [];
    }

    public void RegisterPlugin(IPlugin plugin)
    {
        _plugins.Add(plugin);
    }

    public void UnregisterPlugin(IPlugin plugin)
    {
        _plugins.Remove(plugin);
    }

    internal object? GetExistingInstance(Type type)
    {
        return _plugins.FirstOrDefault(plugin => plugin.GetType().IsAssignableFrom(type));
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