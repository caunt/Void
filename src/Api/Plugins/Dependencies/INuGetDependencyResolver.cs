namespace Void.Proxy.Api.Plugins.Dependencies;

public interface INuGetDependencyResolver : IDependencyResolver
{
    public void AddRepository(string uri);
}
