using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.Plugins.Dependencies.Embedded;
using Void.Proxy.Plugins.Dependencies.Remote.NuGetSource;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyResolver(ILogger logger, AssemblyLoadContext context, Func<AssemblyName, Assembly?> searchInPlugins)
{
    private static readonly string[] VoidDependencies = [nameof(Void)];
    private static readonly string[] SharedDependencies = [nameof(Microsoft)];
    private static readonly string[] SystemDependencies = [nameof(System), "netstandard"];

    private readonly NuGetDependencyResolver _nuget = new(logger);
    private readonly EmbeddedDependencyResolver _embedded = new(logger);
    private readonly AssemblyDependencyResolver _directory = new(Directory.GetCurrentDirectory());
    private readonly Func<AssemblyName, Assembly?>? _searchInPlugins = searchInPlugins;

    public Assembly? Resolve(AssemblyName assemblyName)
    {
        Assembly? assembly = null;

        // try loading embedded assemblies
        if (VoidDependencies.Any(assemblyName.FullName.StartsWith))
        {
            assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name);

            if (assembly is null)
            {
                using var assemblyStream = _embedded.ResolveEmbeddedAssemblyStream(assemblyName);

                if (assemblyStream is not null)
                    assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);
            }
        }

        // try loading platform referenced assemblies
        if (SharedDependencies.Any(assemblyName.FullName.StartsWith) && assembly is null)
        {
            assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name);

            if (assembly is not null && assemblyName.Version is not null)
            {
                var loadedAssemblyName = assembly.GetName();

                if (loadedAssemblyName.Version is not null && loadedAssemblyName.Version.CompareTo(assemblyName.Version) is not 0)
                    logger.LogWarning("In {ContextName} context {AssemblyName} version {AssemblyVersion} mismatch requested {RequestedAssemblyVersion} version", context.Name, loadedAssemblyName.Name, loadedAssemblyName.Version, assemblyName.Version);
            }
        }

        // try loading system assemblies
        if (SystemDependencies.Any(assemblyName.FullName.StartsWith) && assembly is null)
            return AssemblyLoadContext.Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name) ?? AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);

        if (_searchInPlugins is not null && assembly is null)
        {
            // TODO: this is a temporary workaround
            assembly = _searchInPlugins(assemblyName);
        }

        // fallback to local directory and NuGet
        if (assembly is null)
        {
            var assemblyPath = _directory?.ResolveAssemblyToPath(assemblyName) ?? _nuget.ResolveAssemblyPath(assemblyName);

            if (assemblyPath is not null)
                assembly = context.LoadFromAssemblyPath(assemblyPath);
        }

        return assembly;
    }

    public string? ResolveUnmanagedDll(string unmanagedDllName)
    {
        return _directory.ResolveUnmanagedDllToPath(unmanagedDllName);
    }
}
