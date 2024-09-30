using System.Reflection;

namespace Void.Proxy.API.Plugins.Services;

public interface IPluginDependencyService
{
    public string? ResolveAssemblyPath(AssemblyName assemblyName);
    public Stream? ResolveEmbeddedAssemblyStream(AssemblyName assemblyName);
}