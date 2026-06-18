using System.Reflection;
using System.Runtime.Loader;

namespace Void.Proxy.Api.Plugins.Dependencies;

public interface IDependencyResolver
{
    /// <summary>
    /// Resolves the requested assembly for loading into the specified context.
    /// </summary>
    /// <param name="context">The assembly load context that should own the resolved assembly.</param>
    /// <param name="assemblyName">The identity of the assembly to resolve.</param>
    /// <returns>
    /// The loaded <see cref="Assembly"/> when this resolver can satisfy <paramref name="assemblyName"/>; otherwise, <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// Implementations should return <see langword="null"/> when the requested assembly is outside their resolution source or cannot be found.
    /// </remarks>
    public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName);
}
