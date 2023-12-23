namespace Void.Proxy.Plugins.API.Reflection;

public class WeakPluginReference
{
    private readonly PluginLoadContext _context;
    private readonly WeakReference<IPlugin>[] _plugins;

    // do not use primary constructor because they are saving strict reference to parameters
    public WeakPluginReference(PluginLoadContext context, IPlugin[] plugins)
    {
        _context = context;
        _plugins = Array.ConvertAll(plugins, plugin => new WeakReference<IPlugin>(plugin, true));
    }

    public bool IsAlive => _plugins.Any(plugin => plugin.TryGetTarget(out _));

    public PluginLoadContext Context => _context;

    public IPlugin[] Plugins => Array.ConvertAll(_plugins, reference => reference.TryGetTarget(out var plugin) ? plugin : throw new ObjectDisposedException(_context.Name));
}