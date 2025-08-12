using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Context;
using ZLinq;

namespace Void.Proxy.Plugins.Containers;

public class WeakPluginContainer
{
    private readonly List<WeakReference<IPlugin>> _references;

    public IEnumerable<IPlugin> Plugins => GetReferences();
    public bool IsAlive => _references.All(plugin => plugin.TryGetTarget(out _));
    public PluginAssemblyLoadContext Context { get; }
    public CancellationTokenSource CancellationTokenSource { get; } = new();

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "They are saving strict reference to parameters")]
    public WeakPluginContainer(PluginAssemblyLoadContext context, params IPlugin[] plugins)
    {
        Context = context;
        _references = [.. plugins.AsValueEnumerable().Select(plugin => new WeakReference<IPlugin>(plugin, true))];
    }

    public void Add(IPlugin plugin)
    {
        if (Plugins.Any(plugin => plugin.GetType().Assembly != plugin.GetType().Assembly))
            throw new InvalidOperationException($"Plugin {plugin.Name} is not from the same assembly as the others");

        _references.Add(new(plugin, true));
    }

    private IEnumerable<IPlugin> GetReferences()
    {
        foreach (var reference in _references)
        {
            yield return !reference.TryGetTarget(out var plugin)
                ? throw new ObjectDisposedException(Context.Name)
                : plugin;
        }
    }
}
