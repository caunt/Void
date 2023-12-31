﻿using Nito.Disposables.Internals;
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

        foreach (var pluginPath in pluginPaths)
        {
            var context = new PluginLoadContext(pluginPath);

            Proxy.Logger.Information($"Loading {context.Name} plugin");

            var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginPath)));
            var plugins = RegisterPlugins(assembly);

            if (plugins.Length == 0)
                Proxy.Logger.Error($"Couldn't load {context.Name} plugin, not found any IPlugin implementations");

            var listeners = Proxy.Events.RegisterListeners(assembly);

            if (listeners.Length == 0)
                Proxy.Logger.Warning($"Plugin {context.Name} has no event listeners");

            _references.Add(new(context, plugins, listeners));

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

            UnregisterPlugins(reference.Plugins);
            Proxy.Events.UnregisterListeners(reference.Listeners);

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

    internal object? GetExistingInstance(Type type) => _plugins.FirstOrDefault(plugin => plugin.GetType().IsAssignableFrom(type));

    private object? CreatePluginInstance(Type type)
    {
        var instance = Activator.CreateInstance(type);
        var property = type.GetProperty(nameof(IPlugin.Logger));

        var logger = Proxy.Logger.ForContext("SourceContext", type.Name);
        property?.SetValue(instance, logger);

        return instance;
    }
}
