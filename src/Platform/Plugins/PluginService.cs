using System.Diagnostics;
using System.Reflection;
using Nito.Disposables.Internals;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Plugins;
using Void.Proxy.Reflection;
using Void.Proxy.Utils;

namespace Void.Proxy.Plugins;

public class PluginService(ILogger<PluginService> logger, IEventService events, IServiceProvider services, IPluginDependencyService dependencies) : IPluginService
{
    private readonly TimeSpan _gcRate = TimeSpan.FromMilliseconds(500);
    private readonly List<IPlugin> _plugins = [];
    private readonly List<WeakPluginReference> _references = [];
    private readonly TimeSpan _unloadTimeout = TimeSpan.FromSeconds(10);

    public async ValueTask LoadAsync(string path = "plugins", CancellationToken cancellationToken = default)
    {
        var pluginsDirectoryInfo = new DirectoryInfo(path);

        if (!pluginsDirectoryInfo.Exists)
            pluginsDirectoryInfo.Create();

        await ExtractEmbeddedPluginsAsync(path, cancellationToken);

        var pluginPaths = pluginsDirectoryInfo.GetFiles("*.dll").Select(fileInfo => fileInfo.FullName).ToArray();

        logger.LogInformation("Loading {Count} plugins", pluginPaths.Length);

        foreach (var pluginPath in pluginPaths)
        {
            var context = new PluginLoadContext(dependencies, pluginPath);

            logger.LogInformation("Loading {PluginName} plugin", context.Name);

            var plugins = RegisterPlugins(context.Name, context.PluginAssembly);

            if (plugins.Length == 0)
            {
                logger.LogWarning("Plugin {PluginName} has no IPlugin implementations", context.Name);
                continue;
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

    public IPlugin[] RegisterPlugins(string? name, Assembly assembly)
    {
        var pluginInterface = typeof(IPlugin);

        try
        {
            var plugins = assembly.GetTypes().Where(pluginInterface.IsAssignableFrom).Select(CreatePluginInstance).Cast<IPlugin?>().WhereNotNull().ToArray();

            _plugins.AddRange(plugins);
            return plugins;
        }
        catch (ReflectionTypeLoadException exception)
        {
            logger.LogError("Assembly {AssemblyName} cannot be loaded:", name);

            var noStackTrace = exception.LoaderExceptions.WhereNotNull().Where(loaderException => string.IsNullOrWhiteSpace(loaderException.StackTrace)).ToArray();

            if (noStackTrace.Length == exception.LoaderExceptions.Length)
                logger.LogError("{Exceptions}", string.Join(", ", noStackTrace.Select(loaderException => loaderException.Message)));
            else
                logger.LogError(exception, "Multiple Exceptions:");
        }

        return [];
    }

    public void UnregisterPlugins(IPlugin[] plugins)
    {
        foreach (var plugin in plugins)
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

    private async ValueTask ExtractEmbeddedPluginsAsync(string path, CancellationToken cancellationToken)
    {
        var platformAssembly = Assembly.GetExecutingAssembly();
        var buildDate = platformAssembly.GetCustomAttribute<BuildDateAttribute>();

        var embeddedPlugins = platformAssembly.GetManifestResourceNames().Where(name => name.Contains(nameof(Plugins))).Select(embeddedPluginName =>
        {
            var fileName = Path.Combine(path, embeddedPluginName);

            return new
            {
                ResourceName = embeddedPluginName,
                FileName = fileName,
                Exists = File.Exists(fileName)
            };
        }).ToArray();

        var extractedOnceBefore = embeddedPlugins.Any(plugin => plugin.Exists);

        foreach (var plugin in embeddedPlugins)
        {
            var upgrading = false;

            if (plugin.Exists)
            {
                if (buildDate is null)
                    continue;

                var extractionDateTime = File.GetLastWriteTimeUtc(plugin.FileName);

                if (extractionDateTime >= buildDate.DateTime)
                    continue;

                logger.LogInformation("Upgrading {PluginName} plugin with newest build", plugin.ResourceName);
                upgrading = true;
            }

            if (!upgrading && extractedOnceBefore)
            {
                logger.LogWarning("Embedded plugin {PluginName} disappeared", plugin.ResourceName);

                var key = ConsoleKey.None;
                while (key is ConsoleKey.None)
                {
                    logger.LogInformation("Do you want to install it again? [y/n]");

                    var keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key is not ConsoleKey.Y and not ConsoleKey.N)
                        continue;

                    key = keyInfo.Key;
                }

                if (key is ConsoleKey.N)
                    continue;

                logger.LogInformation("Proceeding with installation.");
            }

            if (platformAssembly.GetManifestResourceStream(plugin.ResourceName) is not { } stream)
            {
                logger.LogWarning("Embedded plugin {PluginName} couldn't be extracted", plugin.ResourceName);
                continue;
            }

            logger.LogInformation("Extracting {PluginName} embedded plugin", plugin.ResourceName);

            await using var fileStream = File.OpenWrite(plugin.FileName);
            await stream.CopyToAsync(fileStream, cancellationToken);
        }
    }
}