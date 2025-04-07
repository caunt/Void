using System.Reflection;

namespace Void.Proxy.Api.Plugins;

public interface IPluginDependencyService
{
    public string? ResolveAssemblyPath(AssemblyName assemblyName);
    public Stream? ResolveEmbeddedAssemblyStream(AssemblyName assemblyName);
}
