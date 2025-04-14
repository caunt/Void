using System.Diagnostics.CodeAnalysis;
using Void.Common.Plugins;

namespace Void.Proxy.Plugins.Reflection;

public class WeakPluginReference
{
    private readonly WeakReference<IPlugin>[] _plugins;

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "They are saving strict reference to parameters")]
    public WeakPluginReference(PluginLoadContext context, IPlugin[] plugins)
    {
        Context = context;
        _plugins = Array.ConvertAll(plugins, plugin => new WeakReference<IPlugin>(plugin, true));
    }

    public bool IsAlive => _plugins.Any(plugin => plugin.TryGetTarget(out _));
    public PluginLoadContext Context { get; }

    public IPlugin[] Plugins => Array.ConvertAll(_plugins, reference => reference.TryGetTarget(out var plugin) ? plugin : throw new ObjectDisposedException(Context.Name));
}
