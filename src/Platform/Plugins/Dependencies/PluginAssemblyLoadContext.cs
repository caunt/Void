using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.Plugins.Dependencies.Embedded;
using Void.Proxy.Plugins.Dependencies.Remote.NuGetSource;

namespace Void.Proxy.Plugins.Dependencies;

public class PluginAssemblyLoadContext : AssemblyLoadContext
{
    private static readonly string[] VoidDependencies = [nameof(Void)];
    private static readonly string[] SharedDependencies = [nameof(Microsoft)];
    private static readonly string[] SystemDependencies = [nameof(System), "netstandard"];

    private readonly ILogger<PluginAssemblyLoadContext> _logger;
    private readonly NuGetDependencyResolver _nuget;
    private readonly EmbeddedDependencyResolver _embedded;
    private readonly AssemblyDependencyResolver _directory;
    private readonly Func<AssemblyName, Assembly?>? _searchInPlugins;

    public Assembly PluginAssembly { get; }

    public PluginAssemblyLoadContext(ILogger<PluginAssemblyLoadContext> logger, string assemblyName, Stream assemblyStream, Func<AssemblyName, Assembly?>? searchInPlugins = null) : base(assemblyName, true)
    {
        _logger = logger;
        _searchInPlugins = searchInPlugins;

        _nuget = new NuGetDependencyResolver(logger);
        _embedded = new EmbeddedDependencyResolver(logger);
        _directory = new AssemblyDependencyResolver(Directory.GetCurrentDirectory());

        PluginAssembly = LoadFromStream(assemblyStream);
    }

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
                using var assemblyStream = _embedded.ResolveEmbeddedAssemblyStream(assemblyName);

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

        // fallback to local directory and NuGet
        if (assembly is null)
        {
            var assemblyPath = _directory?.ResolveAssemblyToPath(assemblyName) ?? _nuget.ResolveAssemblyPath(assemblyName);

            if (assemblyPath is not null)
                assembly = LoadFromAssemblyPath(assemblyPath);
        }

        if (_searchInPlugins is not null && assembly is null)
        {
            // TODO: this is a temporary workaround
            assembly = _searchInPlugins(assemblyName);
        }

        // sorry, but where am I supposed to find your dependency?
        // throw is mandatory to prevent search in Default context
        if (assembly is null)
            throw new FileNotFoundException("Unable to resolve requested dependency");

        return assembly;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _directory?.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath is null ? nint.Zero : LoadUnmanagedDllFromPath(libraryPath);
    }
}
