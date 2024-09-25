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

    public PluginLoadContext(IPluginDependencyService dependencies, string pluginPath) : base(Path.GetFileName(pluginPath), true)
    {
        _dependencies = dependencies;
        _localResolver = new AssemblyDependencyResolver(pluginPath);
        PluginAssembly = LoadFromAssemblyPath(pluginPath);
    }

    public Assembly PluginAssembly { get; }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        if (VersionedDependencies.Any(assemblyName.FullName.StartsWith))
        {
            var loadedAssembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.FullName == assemblyName.FullName);

            if (loadedAssembly is not null)
                return loadedAssembly;

            // version mismatch here
        }

        if (SharedDependencies.Any(assemblyName.FullName.StartsWith) || SystemDependencies.Any(assemblyName.FullName.StartsWith))
        {
            var loadedAssembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name);

            if (loadedAssembly is not null)
                return loadedAssembly;
        }

        if (SystemDependencies.Any(assemblyName.FullName.StartsWith))
        {
            var loadedAssembly = Default.Assemblies.FirstOrDefault(loadedAssembly => loadedAssembly.GetName().Name == assemblyName.Name);

            // if System dependency still not loaded, load it manually
            return loadedAssembly ?? Default.LoadFromAssemblyName(assemblyName);
        }

        // fallback to local folder and NuGet
        var assembly = _localResolver.ResolveAssemblyToPath(assemblyName) switch
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