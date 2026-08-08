namespace Void.Proxy.Api.Plugins.Dependencies;

/// <summary>
/// Resolves managed and unmanaged plugin dependencies from files.
/// </summary>
public interface IFileDependencyResolver : IDependencyResolver
{
    /// <summary>
    /// Resolves the path of an unmanaged library.
    /// </summary>
    /// <param name="unmanagedDllName">The unmanaged library name requested by the runtime.</param>
    /// <returns>The resolved absolute or relative library path, or <see langword="null" /> when this resolver cannot satisfy the request.</returns>
    public string? ResolveUnmanagedDllToPath(string unmanagedDllName);
}
