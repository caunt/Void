using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Reflection;
using Nito.AsyncEx;
using Nito.Disposables.Internals;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Containers;
using Void.Proxy.Plugins.Context;

namespace Void.Proxy.Plugins;

public class PluginService(ILogger<PluginService> logger, IEventService events, IDependencyService dependencies, InvocationContext context) : IPluginService
{
    private static readonly Option<string[]> _pluginsOption = new Option<string[]>(["--plugin", "-p"], "Provides a path to file, directory or url to load plugin.");

    private readonly AsyncLock _lock = new();
    private readonly List<WeakPluginContainer> _containers = [];
    private readonly TimeSpan _gcRate = TimeSpan.FromMilliseconds(500);
    private readonly TimeSpan _unloadTimeout = TimeSpan.FromSeconds(10);

    public IEnumerable<IPlugin> All => _containers.SelectMany(container => container.Plugins);

    public IEnumerable<string> Containers => _containers.Select(container => container.Context.Name!);

    public static void RegisterOptions(Command command)
    {
        command.AddOption(_pluginsOption);
    }

    public async ValueTask LoadPluginsAsync(string path = "plugins", CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading environment plugins");
        await LoadEnvironmentPluginsAsync(cancellationToken);

        logger.LogInformation("Loading embedded plugins");
        await LoadEmbeddedPluginsAsync(cancellationToken);

        logger.LogInformation("Loading directory plugins");
        await LoadDirectoryPluginsAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask LoadEnvironmentPluginsAsync(CancellationToken cancellationToken = default)
    {
        var plugins = await GetArgumentsPlugins().Concat(GetVariablesPlugins()).Select(async variable =>
        {
            if (File.Exists(variable))
            {
                var name = Path.GetFileName(variable);
                logger.LogTrace("Found {Name} local plugin", name);

                await using var stream = File.OpenRead(variable);
                return LoadContainer(name, stream);
            }
            if (Directory.Exists(variable))
            {
                return await LoadDirectoryPluginTypesAsync(new DirectoryInfo(variable), cancellationToken);
            }
            else if (Uri.TryCreate(variable, UriKind.Absolute, out var url))
            {
                var name = url.LocalPath;
                logger.LogTrace("Found {Name} local plugin", name);

                using var response = await new HttpClient().GetAsync(url);
                await using var stream = response.Content.ReadAsStream();
                return LoadContainer(name, stream);
            }
            else
            {
                return null;
            }
        }).WhenAll();

        await LoadPluginsAsync(plugins.WhereNotNull().SelectMany(x => x), cancellationToken);

        string[] GetArgumentsPlugins()
        {
            return context.ParseResult.GetValueForOption(_pluginsOption) ?? [];
        }

        static string[] GetVariablesPlugins()
        {
            var args = Environment.GetEnvironmentVariable("PLUGINS");

            if (string.IsNullOrWhiteSpace(args))
                return [];

            return args.Split(',', ';');
        }
    }

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

    public async ValueTask LoadDirectoryPluginsAsync(string path = "plugins", CancellationToken cancellationToken = default)
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

        var plugins = await LoadDirectoryPluginTypesAsync(pluginsDirectoryInfo, cancellationToken);
        await LoadPluginsAsync(plugins, cancellationToken);
    }

    public async ValueTask<IEnumerable<Type>> LoadDirectoryPluginTypesAsync(DirectoryInfo directoryInfo, CancellationToken cancellationToken = default)
    {
        return await directoryInfo.GetFiles("*.dll")
            .Select(fileInfo => fileInfo.FullName)
            .Select(async fileName =>
            {
                logger.LogTrace("Found {Name} plugin", Path.GetFileName(fileName));

                await using var stream = File.OpenRead(fileName);
                return LoadContainer(Path.GetFileName(fileName), stream);
            })
            .WhenAll()
            .ContinueWith(task => task.Result.SelectMany(plugins => plugins));
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

        var plugin = dependencies.CreateInstance<IPlugin>(pluginType, container.CancellationTokenSource.Token);

        logger.LogTrace("Loading {Name} plugin", plugin.Name);

        using (var _ = await _lock.LockAsync(cancellationToken))
        {
            // Add to container before events, so listeners of plugin loading event can resolve configs with that plugin assembly
            container.Add(plugin);

            await events.ThrowAsync(new PluginLoadingEvent(plugin), cancellationToken);
            await events.ThrowAsync(new PluginLoadedEvent(plugin), cancellationToken);
        }

        logger.LogInformation("Loaded {Name} plugin from {AssemblyName} ", pluginType.Name, container.Context.PluginAssembly.GetName().Name);
    }

    public IEnumerable<Type> LoadContainer(string name, Stream stream, bool ignoreEmpty = false)
    {
        logger.LogTrace("Loading {Name} plugins", name);
        var context = dependencies.CreateInstance<PluginAssemblyLoadContext>(default, name, stream, _containers.AsReadOnly());

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
            logger.LogError(exception, "Assembly {Name} cannot be loaded:", assemblyName);

            // TODO: prints empty exception?
            // var noStackTrace = exception.LoaderExceptions.WhereNotNull().Where(loaderException => string.IsNullOrWhiteSpace(loaderException.StackTrace)).ToArray();
            // 
            // if (noStackTrace.Length == exception.LoaderExceptions.Length)
            //     logger.LogError("{Exceptions}", string.Join(", ", noStackTrace.Select(loaderException => loaderException.Message)));
            // else
            //     logger.LogError(exception, "Multiple Exceptions:");
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

        var count = container.Plugins.Count();

        logger.LogTrace("Unloading {ContextName} {Count} plugin(s)", name, count);

        using (var _ = await _lock.LockAsync(cancellationToken))
        {
            foreach (var plugin in container.Plugins)
            {
                logger.LogTrace("Unloading {PluginName} plugin", plugin.Name);
                await events.ThrowAsync(new PluginUnloadingEvent(plugin), cancellationToken);
                await events.ThrowAsync(new PluginUnloadedEvent(plugin), cancellationToken);
            }
        }

        events.UnregisterListeners(container.Plugins.Cast<IEventListener>());

        container.CancellationTokenSource.Cancel();
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
