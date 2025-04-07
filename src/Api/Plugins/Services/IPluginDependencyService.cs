using System.Reflection;

namespace Void.Proxy.Api.Plugins.Services;

public interface IPluginDependencyService
{
    public string? ResolveAssemblyPath(AssemblyName assemblyName);
    public Stream? ResolveEmbeddedAssemblyStream(AssemblyName assemblyName);
}
