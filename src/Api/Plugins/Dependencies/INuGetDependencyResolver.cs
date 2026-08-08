namespace Void.Proxy.Api.Plugins.Dependencies;

/// <summary>
/// Resolves plugin dependencies from configured NuGet package repositories.
/// </summary>
public interface INuGetDependencyResolver : IDependencyResolver
{
    /// <summary>
    /// Adds a package repository to the resolution sources.
    /// </summary>
    /// <param name="uri">The repository service-index URI.</param>
    public void AddRepository(string uri);
}
