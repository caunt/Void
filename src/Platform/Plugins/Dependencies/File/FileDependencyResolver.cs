using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Dependencies.File;

public class FileDependencyResolver(AssemblyDependencyResolver resolver) : IFileDependencyResolver
{
    public static FileDependencyResolver Factory(IServiceProvider provider)
    {
        return ActivatorUtilities.CreateInstance<FileDependencyResolver>(provider, new AssemblyDependencyResolver(Directory.GetCurrentDirectory()));
    }

    public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var path = resolver.ResolveAssemblyToPath(assemblyName);

        if (path is null)
            return null;

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return context.LoadFromStream(stream);
    }

    public string? ResolveUnmanagedDllToPath(string unmanagedDllName)
    {
        return resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
    }
}
