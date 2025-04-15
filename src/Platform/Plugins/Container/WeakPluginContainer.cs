using System.Diagnostics.CodeAnalysis;
using Void.Common.Plugins;
using Void.Proxy.Plugins.Context;

namespace Void.Proxy.Plugins.Container;

public class WeakPluginContainer
{
    private readonly WeakReference<IPlugin>[] _plugins;

    public bool IsAlive => _plugins.Any(plugin => plugin.TryGetTarget(out _));
    public PluginAssemblyLoadContext Context { get; }

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "They are saving strict reference to parameters")]
    public WeakPluginContainer(PluginAssemblyLoadContext context, IPlugin[] plugins)
    {
        Context = context;
        _plugins = Array.ConvertAll(plugins, plugin => new WeakReference<IPlugin>(plugin, true));
    }

    public IPlugin[] Plugins => Array.ConvertAll(_plugins, reference => reference.TryGetTarget(out var plugin) ? plugin : throw new ObjectDisposedException(Context.Name));
}
