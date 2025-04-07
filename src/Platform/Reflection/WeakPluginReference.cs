using Void.Common;

namespace Void.Proxy.Reflection;

public class WeakPluginReference
{
    private readonly WeakReference<IPlugin>[] _plugins;

    // Do not use primary constructor because they are saving strict reference to parameters
    public WeakPluginReference(PluginLoadContext context, IPlugin[] plugins)
    {
        Context = context;
        _plugins = Array.ConvertAll(plugins, plugin => new WeakReference<IPlugin>(plugin, true));
    }

    public bool IsAlive => _plugins.Any(plugin => plugin.TryGetTarget(out _));
    public PluginLoadContext Context { get; }

    public IPlugin[] Plugins => Array.ConvertAll(_plugins, reference => reference.TryGetTarget(out var plugin) ? plugin : throw new ObjectDisposedException(Context.Name));
}
