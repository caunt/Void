using System.Reflection;
using System.Runtime.Loader;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Dependencies.Embedded;

public class EmbeddedDependencyResolver(ILogger<EmbeddedDependencyResolver> logger) : IEmbeddedDependencyResolver
{
    public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        logger.LogTrace("Resolving {AssemblyName} embedded dependency", assemblyName.Name);

        if (string.IsNullOrWhiteSpace(assemblyName.Name))
            return null;

        var assembly = Assembly.GetExecutingAssembly();
        var resource = assembly.GetManifestResourceNames().FirstOrDefault(name => name.StartsWith(assemblyName.Name));

        if (string.IsNullOrWhiteSpace(resource))
            return null;

        if (assembly.GetManifestResourceStream(resource) is { } stream)
            return context.LoadFromStream(stream);

        logger.LogWarning("Embedded assembly {ResourceName} couldn't be loaded", resource);
        return null;
    }
}
