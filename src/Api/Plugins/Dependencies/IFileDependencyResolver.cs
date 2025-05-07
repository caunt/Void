namespace Void.Proxy.Api.Plugins.Dependencies;

public interface IFileDependencyResolver : IDependencyResolver
{
    public string? ResolveUnmanagedDllToPath(string unmanagedDllName);
}
