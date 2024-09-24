using System.Reflection;
using System.Runtime.Loader;

namespace Void.Proxy.Reflection;

public class PluginLoadContext(string pluginPath) : AssemblyLoadContext(Path.GetFileName(pluginPath), true)
{
    private static readonly string[] SharedDependencies = [nameof(Void), nameof(Microsoft), nameof(System)];
    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (SharedDependencies.Any(prefix => !string.IsNullOrWhiteSpace(assemblyName.Name) && assemblyName.Name.StartsWith(prefix)))
        {
            var sharedAssembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.FullName == assemblyName.FullName);

            if (sharedAssembly is not null)
                return sharedAssembly;
        }

        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

        if (assemblyPath is not null) 
            return LoadFromAssemblyPath(assemblyPath);

        // TODO: implement NuGet resolver here
        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}