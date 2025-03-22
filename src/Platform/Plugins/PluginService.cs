using Nito.Disposables.Internals;
using System.Diagnostics;
using System.Reflection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Plugins.Services;
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
            logger.LogTrace("Found {ResourceName} embedded plugin", resourceName);

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
        {
            try
            {
                pluginsDirectoryInfo.Create();
            }
            catch (UnauthorizedAccessException exception)
            {
                logger.LogWarning("Cannot create {Path} plugins path: {Message}", path, exception.Message);
            }

            return;
        }

        var pluginsFiles = pluginsDirectoryInfo.GetFiles("*.dll").Select(fileInfo => fileInfo.FullName);

        foreach (var pluginPath in pluginsFiles)
        {
            logger.LogTrace("Found {ResourceName} directory plugin", Path.GetFileName(pluginPath));

            await using var stream = File.OpenRead(pluginPath);
            await LoadPluginsAsync(Path.GetFileName(pluginPath), stream, cancellationToken);
        }
    }

    public async ValueTask LoadPluginsAsync(string assemblyName, Stream assemblyStream, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Loading {AssemblyName} plugins", assemblyName);

        var context = new PluginLoadContext(services.GetRequiredService<ILogger<PluginLoadContext>>(), dependencies, assemblyName, assemblyStream);
        var plugins = GetPlugins(assemblyName, context.PluginAssembly);

        foreach (var plugin in plugins)
            RegisterPlugin(plugin);

        if (plugins.Length == 0)
        {
            logger.LogWarning("Plugin {PluginName} has no IPlugin implementations", context.Name);
            return;
        }

        events.RegisterListeners(plugins.Cast<IEventListener>());
        _references.Add(new WeakPluginReference(context, plugins));

        foreach (var plugin in plugins)
            await events.ThrowAsync(new PluginLoadEvent(plugin), cancellationToken);

        logger.LogInformation("Loaded {Count} plugin(s) from {AssemblyName} ", plugins.Length, assemblyName);
    }

    public async ValueTask UnloadPluginsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Unloading all plugins");

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
            var count = reference.Plugins.Length;

            logger.LogTrace("Unloading {ContextName} {Count} plugin(s)", name, count);

            foreach (var plugin in reference.Plugins)
            {
                await events.ThrowAsync(new PluginUnloadEvent(plugin), cancellationToken);
                UnregisterPlugin(plugin);
            }

            events.UnregisterListeners(reference.Plugins.Cast<IEventListener>());

            reference.Context.Unload();

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
                throw new Exception($"Plugins context {name} refuses to unload");

            logger.LogInformation("Unloaded {ContextName} {Count} plugin(s)", name, count);
        }

        _references.RemoveAll(reference => !reference.IsAlive);
    }

    public IPlugin[] GetPlugins(string assemblyName, Assembly assembly)
    {
        logger.LogTrace("Searching for IPlugin interfaces in {AssemblyName}", assemblyName);

        var pluginInterface = typeof(IPlugin);

        try
        {
            var plugins = assembly.GetTypes().Where(pluginInterface.IsAssignableFrom).Select(type => ActivatorUtilities.CreateInstance(services, type)).Cast<IPlugin?>().WhereNotNull().ToArray();
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
        logger.LogTrace("Registering {PluginName} plugin", plugin.Name);
        _plugins.Add(plugin);
    }

    public void UnregisterPlugin(IPlugin plugin)
    {
        logger.LogTrace("Unregistering {PluginName} plugin", plugin.Name);
        _plugins.Remove(plugin);
    }
}