using System.Reflection;
using System.Runtime.Loader;

namespace Void.Proxy.Api.Plugins.Dependencies;

public interface IDependencyResolver
{
    public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName);
}
