using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Reflection;

public class PluginLoadContext : AssemblyLoadContext
{
    private static readonly string[] VoidDependencies = [nameof(Void)];
    private static readonly string[] SharedDependencies = [nameof(Microsoft)];
    private static readonly string[] SystemDependencies = [nameof(System), "netstandard"];
    private readonly IPluginDependencyService _dependencies;
    private readonly AssemblyDependencyResolver? _localDependencies;

    private readonly ILogger<PluginLoadContext> _logger;

    public PluginLoadContext(ILogger<PluginLoadContext> logger, IPluginDependencyService dependencies, string assemblyName, Stream assemblyStream, string? componentAssemblyPath = null) : base(assemblyName, true)
    {
        _logger = logger;
        _dependencies = dependencies;

        if (!string.IsNullOrWhiteSpace(componentAssemblyPath))
            _localDependencies = new AssemblyDependencyResolver(componentAssemblyPath);

        PluginAssembly = LoadFromStream(assemblyStream);
    }

    public Assembly PluginAssembly { get; }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        Assembly? assembly = null;

        _logger.LogTrace("Loading {AssemblyName} assembly into {ContextName} context", assemblyName.Name, Name);

        // try loading embedded assemblies
        if (VoidDependencies.Any(assemblyName.FullName.StartsWith))
        {
            assembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.FullName == assemblyName.FullName);

            if (assembly is null)
            {
                using var assemblyStream = _dependencies.ResolveEmbeddedAssemblyStream(assemblyName);

                if (assemblyStream is not null)
                    assembly = Default.LoadFromStream(assemblyStream);
            }
        }

        // try loading platform referenced assemblies
        if (SharedDependencies.Any(assemblyName.FullName.StartsWith) && assembly is null)
        {
            assembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name);

            if (assembly is not null && assemblyName.Version is not null)
            {
                var loadedAssemblyName = assembly.GetName();

                if (loadedAssemblyName.Version is not null && loadedAssemblyName.Version.CompareTo(assemblyName.Version) is not 0)
                    _logger.LogWarning("In {ContextName} context {AssemblyName} version {AssemblyVersion} mismatch requested {RequestedAssemblyVersion} version", Name, loadedAssemblyName.Name, loadedAssemblyName.Version, assemblyName.Version);
            }
        }

        // try loading system assemblies
        if (SystemDependencies.Any(assemblyName.FullName.StartsWith) && assembly is null)
            return Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name) ?? Default.LoadFromAssemblyName(assemblyName);

        // fallback to local folder and NuGet
        if (assembly is null)
        {
            var assemblyPath = _localDependencies?.ResolveAssemblyToPath(assemblyName) ?? _dependencies.ResolveAssemblyPath(assemblyName);

            if (assemblyPath is not null)
                assembly = LoadFromAssemblyPath(assemblyPath);
        }

        // sorry, but where am I supposed to find your dependency?
        // throw is mandatory to prevent search in Default context
        if (assembly is null)
            throw new FileNotFoundException("Unable to resolve requested dependency");

        return assembly;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _localDependencies?.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(libraryPath);
    }
}
