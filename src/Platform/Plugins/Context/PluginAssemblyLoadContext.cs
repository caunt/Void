using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Context;

public class PluginAssemblyLoadContext : AssemblyLoadContext
{
    private readonly ILogger<PluginAssemblyLoadContext> _logger;
    private readonly DependencyResolver _resolver;

    public Assembly PluginAssembly { get; }

    public PluginAssemblyLoadContext(ILogger<PluginAssemblyLoadContext> logger, string assemblyName, Stream assemblyStream, Func<AssemblyName, Assembly?> searchInPlugins) : base(assemblyName, true)
    {
        _logger = logger;
        _resolver = new DependencyResolver(logger, this, searchInPlugins);

        PluginAssembly = LoadFromStream(assemblyStream);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        _logger.LogTrace("Loading {AssemblyName} assembly into {ContextName} context", assemblyName.Name, Name);

        var assembly = _resolver.Resolve(assemblyName) ??
            throw new FileNotFoundException("Unable to resolve requested dependency");

        // Sorry, but where am I supposed to find your dependency?
        // Throw is mandatory to prevent unwanted search in Default context

        return assembly;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        if (_resolver.ResolveUnmanagedDll(unmanagedDllName) is not { } libraryPath)
            return nint.Zero;

        return LoadUnmanagedDllFromPath(libraryPath);
    }
}
