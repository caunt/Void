using System.Reflection;

namespace Void.Proxy.Plugins.Dependencies.Embedded;

public class EmbeddedDependencyResolver(ILogger logger)
{
    public Stream? ResolveEmbeddedAssemblyStream(AssemblyName assemblyName)
    {
        logger.LogTrace("Resolving {AssemblyName} embedded dependency", assemblyName.Name);

        if (string.IsNullOrWhiteSpace(assemblyName.Name))
            return null;

        var assembly = Assembly.GetExecutingAssembly();
        var resource = assembly.GetManifestResourceNames().FirstOrDefault(name => name.StartsWith(assemblyName.Name));

        if (string.IsNullOrWhiteSpace(resource))
            return null;

        if (assembly.GetManifestResourceStream(resource) is { } stream)
            return stream;

        logger.LogWarning("Embedded assembly {ResourceName} couldn't be loaded", resource);
        return null;
    }
}
