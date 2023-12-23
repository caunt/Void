using Void.Proxy.API.Events;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Reflection;

public class WeakPluginReference
{
    public bool IsAlive => _plugins.Any(plugin => plugin.TryGetTarget(out _));
    public PluginLoadContext Context => _context;
    public IPlugin[] Plugins => Array.ConvertAll(_plugins, reference => reference.TryGetTarget(out var plugin) ? plugin : throw new ObjectDisposedException(_context.Name));
    public IEventListener[] Listeners => Array.ConvertAll(_listeners, reference => reference.TryGetTarget(out var listener) ? listener : throw new ObjectDisposedException(_context.Name));

    private readonly PluginLoadContext _context;
    private readonly WeakReference<IPlugin>[] _plugins;
    private readonly WeakReference<IEventListener>[] _listeners;

    // do not use primary constructor because they are saving strict reference to parameters
    public WeakPluginReference(PluginLoadContext context, IPlugin[] plugins, IEventListener[] listeners)
    {
        _context = context;
        _plugins = Array.ConvertAll(plugins, plugin => new WeakReference<IPlugin>(plugin, true));
        _listeners = Array.ConvertAll(listeners, listener => new WeakReference<IEventListener>(listener, true));
    }
}