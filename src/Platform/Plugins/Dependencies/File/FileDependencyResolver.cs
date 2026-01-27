using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;
using Void.Proxy.Api;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Dependencies.File;

public class FileDependencyResolver(IRunOptions runOptions) : IFileDependencyResolver
{
    public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        if (assemblyName.Name is null)
            return null;

        var path = ResolveAssemblyToPath(assemblyName.Name);

        if (path is null)
            return null;

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        if (!IsManagedDotNetAssembly(stream))
            return null;

        return context.LoadFromStream(stream);
    }

    public string? ResolveUnmanagedDllToPath(string unmanagedDllName)
    {
        return ResolveAssemblyToPath(unmanagedDllName);
    }

    private string? ResolveAssemblyToPath(string name)
    {
        var searchName = $"{name}.dll";
        var candidates = Directory.EnumerateFiles(runOptions.WorkingDirectory, searchName);

        var pluginsDirectory = Path.Combine(runOptions.WorkingDirectory, PluginService.DefaultPluginsPath);

        if (Directory.Exists(pluginsDirectory))
            candidates = candidates.Concat(Directory.EnumerateFiles(pluginsDirectory, searchName));

        return candidates.SingleOrDefault();
    }

    private static bool IsManagedDotNetAssembly(Stream stream)
    {
        try
        {
            using var reader = new PEReader(stream, PEStreamOptions.LeaveOpen);
            return reader is { PEHeaders.CorHeader: not null, HasMetadata: true };
        }
        finally
        {
            stream.Position = 0;
        }
    }
}
