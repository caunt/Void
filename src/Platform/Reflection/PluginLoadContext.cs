using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Reflection;

public class PluginLoadContext : AssemblyLoadContext
{
    private static readonly string[] VersionedDependencies = [nameof(Void)];
    private static readonly string[] SharedDependencies = [nameof(Microsoft)];
    private static readonly string[] SystemDependencies = [nameof(System), "netstandard"];
    private readonly IPluginDependencyService _dependencies;
    private readonly AssemblyDependencyResolver _localResolver;

    private readonly ILogger<PluginLoadContext> _logger;

    public PluginLoadContext(ILogger<PluginLoadContext> logger, IPluginDependencyService dependencies, string pluginPath) : base(Path.GetFileName(pluginPath), true)
    {
        _logger = logger;
        _dependencies = dependencies;

        _localResolver = new AssemblyDependencyResolver(pluginPath);
        PluginAssembly = LoadFromAssemblyPath(pluginPath);
    }

    public Assembly PluginAssembly { get; }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        Assembly? assembly = null;

        _logger.LogTrace("Loading {AssemblyName} assembly into {ContextName} context", assemblyName.Name, Name);

        if (VersionedDependencies.Any(assemblyName.FullName.StartsWith))
            assembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.FullName == assemblyName.FullName);

        if (SharedDependencies.Any(assemblyName.FullName.StartsWith) && assembly is null)
            assembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name);

        if (assembly is not null)
        {
            if (assemblyName.Version is not null)
            {
                var loadedAssemblyName = assembly.GetName();

                if (loadedAssemblyName.Version is not null && loadedAssemblyName.Version.CompareTo(assemblyName.Version) is not 0)
                    _logger.LogWarning("In {ContextName} context {AssemblyName} version {AssemblyVersion} mismatch requested {RequestedAssemblyVersion} version", Name, loadedAssemblyName.Name, loadedAssemblyName.Version, assemblyName.Version);
            }

            return assembly;
        }

        if (SystemDependencies.Any(assemblyName.FullName.StartsWith) && assembly is null)
            return Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name) ?? Default.LoadFromAssemblyName(assemblyName);

        // fallback to local folder and NuGet
        assembly = _localResolver.ResolveAssemblyToPath(assemblyName) switch
        {
            { } assemblyPath => LoadFromAssemblyPath(assemblyPath),
            _ when _dependencies.ResolveAssemblyPath(assemblyName) is { } assemblyPath => LoadFromAssemblyPath(assemblyPath),
            _ => null
        };

        // sorry, but where am I supposed to find your dependency?
        // throw is mandatory to prevent search in Default context
        if (assembly is null)
            throw new FileNotFoundException("Unable to resolve requested dependency");

        return assembly;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _localResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}