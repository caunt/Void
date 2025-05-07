using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Containers;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyResolver(
    ILogger<DependencyResolver> logger,
    INuGetDependencyResolver nuget,
    IEmbeddedDependencyResolver embedded,
    IFileDependencyResolver file,
    AssemblyLoadContext context,
    IReadOnlyCollection<WeakPluginContainer> containers)
{
    private static readonly string[] VoidDependencies = [nameof(Void)];
    private static readonly string[] SharedDependencies = [nameof(Microsoft)];
    private static readonly string[] SystemDependencies = [nameof(System), "netstandard"];

    public Assembly? Resolve(AssemblyName assemblyName)
    {
        return
            ResolveStandardAssembly(assemblyName) ??
            ResolveContainerAssembly(assemblyName) ??
            file.Resolve(context, assemblyName) ??
            nuget.Resolve(context, assemblyName);
    }

    public string? ResolveUnmanagedDll(string unmanagedDllName)
    {
        return file.ResolveUnmanagedDllToPath(unmanagedDllName);
    }

    public Assembly? ResolveContainerAssembly(AssemblyName assemblyName)
    {
        foreach (var reference in containers)
        {
            var assembly = reference.Context.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.FullName == assemblyName.FullName);

            if (assembly is null)
                continue;

            return assembly;
        }

        return null;
    }

    public Assembly ResolveStandardAssembly(AssemblyName assemblyName)
    {
        Assembly? assembly = null;

        // try loading embedded assemblies
        if (VoidDependencies.Any(assemblyName.FullName.StartsWith))
        {
            assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name);
            assembly ??= embedded.Resolve(AssemblyLoadContext.Default, assemblyName);
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

        return assembly;
    }
}
