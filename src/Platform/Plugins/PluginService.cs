using Nito.AsyncEx;
using Nito.Disposables.Internals;
using System.Diagnostics;
using System.Reflection;
using Void.Common.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Container;
using Void.Proxy.Plugins.Context;

namespace Void.Proxy.Plugins;

public class PluginService(ILogger<PluginService> logger, IPlayerService players, IEventService events, IDependencyService dependencies) : IPluginService
{
    private readonly List<WeakPluginContainer> _containers = [];
    private readonly TimeSpan _gcRate = TimeSpan.FromMilliseconds(500);
    private readonly TimeSpan _unloadTimeout = TimeSpan.FromSeconds(10);

    public IEnumerable<IPlugin> All => _containers.SelectMany(container => container.Plugins);

    public IEnumerable<string> Containers => _containers.Select(container => container.Context.Name!);

    public async ValueTask LoadEmbeddedPluginsAsync(CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames().Where(name => name.Contains(nameof(Plugins)));

        var plugins = resources.SelectMany(name =>
        {
            logger.LogTrace("Found {Name} embedded plugin", name);

            if (assembly.GetManifestResourceStream(name) is not { } stream)
            {
                logger.LogWarning("Embedded plugin {Name} couldn't be loaded", name);
                return [];
            }

            try
            {
                return LoadContainer(name, stream, ignoreEmpty: true);
            }
            finally
            {
                stream.Close();
            }
        });

        await LoadPluginsAsync(plugins, cancellationToken);
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

        var plugins = await pluginsDirectoryInfo.GetFiles("*.dll")
            .Select(fileInfo => fileInfo.FullName)
            .Select(async fileName =>
            {
                logger.LogTrace("Found {Name} plugin", Path.GetFileName(fileName));

                await using var stream = File.OpenRead(fileName);
                return LoadContainer(Path.GetFileName(fileName), stream);
            })
            .WhenAll()
            .ContinueWith(task => task.Result.SelectMany(plugins => plugins));

        await LoadPluginsAsync(plugins, cancellationToken);
    }

    public async ValueTask LoadPluginsAsync(IEnumerable<Type> plugins, CancellationToken cancellationToken = default)
    {
        plugins = plugins.OrderByDescending(plugin => plugin.IsAssignableTo(typeof(IApiPlugin)));

        foreach (var plugin in plugins)
            await LoadPluginAsync(plugin, cancellationToken);
    }

    public async ValueTask LoadPluginAsync(Type pluginType, CancellationToken cancellationToken = default)
    {
        var container = _containers.FirstOrDefault(reference => reference.Context.PluginAssembly == pluginType.Assembly) ??
            throw new Exception($"No container found for {pluginType.Name} plugin");

        var plugin = dependencies.CreateInstance<IPlugin>(pluginType);

        logger.LogTrace("Loading {Name} plugin", plugin.Name);
        await events.ThrowAsync(new PluginLoadEvent(plugin), cancellationToken);

        container.Add(plugin);
        logger.LogInformation("Loaded {Name} plugin from {AssemblyName} ", pluginType.Name, container.Context.PluginAssembly.GetName().Name);
    }

    public IEnumerable<Type> LoadContainer(string name, Stream stream, bool ignoreEmpty = false)
    {
        logger.LogTrace("Loading {Name} plugins", name);
        var context = new PluginAssemblyLoadContext(dependencies, name, stream, searchInPlugins: (assemblyName) =>
        {
            foreach (var reference in _containers)
            {
                var assembly = reference.Context.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.FullName == assemblyName.FullName);

                if (assembly is null)
                    continue;

                return assembly;
            }

            return null;
        });

        var plugins = GetPlugins(context.PluginAssembly);

        if (!plugins.Any())
        {
            logger.Log(ignoreEmpty ? LogLevel.Trace : LogLevel.Warning, "Plugin {ContextName} has no IPlugin implementations", context.Name);
            return plugins;
        }

        _containers.Add(new WeakPluginContainer(context));

        return plugins;
    }

    public IEnumerable<Type> GetPlugins(Assembly assembly)
    {
        var assemblyName = assembly.GetName().Name;

        logger.LogTrace("Looking for IPlugin interfaces in {Name}", assemblyName);

        try
        {
            var plugins = assembly.GetTypes()
                .Where(typeof(IPlugin).IsAssignableFrom)
                .Where(type => !type.IsAbstract);

            return plugins;
        }
        catch (ReflectionTypeLoadException exception)
        {
            logger.LogError("Assembly {Name} cannot be loaded:", assemblyName);

            var noStackTrace = exception.LoaderExceptions.WhereNotNull().Where(loaderException => string.IsNullOrWhiteSpace(loaderException.StackTrace)).ToArray();

            if (noStackTrace.Length == exception.LoaderExceptions.Length)
                logger.LogError("{Exceptions}", string.Join(", ", noStackTrace.Select(loaderException => loaderException.Message)));
            else
                logger.LogError(exception, "Multiple Exceptions:");
        }

        return [];
    }

    public async ValueTask UnloadContainersAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Unloading all plugins");
        await _containers.Select(async reference => await UnloadContainerAsync(reference.Context.Name!, cancellationToken)).WhenAll();
    }

    public async ValueTask UnloadContainerAsync(string name, CancellationToken cancellationToken = default)
    {
        var container = _containers.FirstOrDefault(reference => reference.Context.Name == name) ??
            throw new Exception($"Plugin container {name} not found");

        if (!container.IsAlive)
            throw new Exception("Plugin context already unloaded");

        foreach (var player in players.All)
            player.Context.Services.RemoveServicesByAssembly(container.Context.PluginAssembly);

        var count = container.Plugins.Count();

        logger.LogTrace("Unloading {ContextName} {Count} plugin(s)", name, count);

        foreach (var plugin in container.Plugins)
        {
            logger.LogTrace("Unloading {PluginName} plugin", plugin.Name);
            await events.ThrowAsync(new PluginUnloadEvent(plugin), cancellationToken);
        }

        events.UnregisterListeners(container.Plugins.Cast<IEventListener>());

        container.Context.Unload();

        var collectionTime = Stopwatch.GetTimestamp();

        do
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (!container.IsAlive)
                break;

            await Task.Delay(_gcRate, cancellationToken);
        } while (Stopwatch.GetElapsedTime(collectionTime) < _unloadTimeout);

        if (container.IsAlive)
            throw new Exception($"Plugins context {name} refuses to unload");

        logger.LogInformation("Unloaded {ContextName} {Count} plugin(s)", name, count);

        _containers.RemoveAll(reference => !reference.IsAlive);
    }
}
