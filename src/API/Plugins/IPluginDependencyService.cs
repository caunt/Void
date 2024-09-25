using System.Reflection;

namespace Void.Proxy.API.Plugins;

public interface IPluginDependencyService
{
    public string? ResolveAssemblyPath(AssemblyName assemblyName);
}