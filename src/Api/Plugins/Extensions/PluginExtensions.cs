using System.Diagnostics.CodeAnalysis;

namespace Void.Proxy.Api.Plugins.Extensions;

/// <summary>
/// Provides assembly-based plugin lookup operations.
/// </summary>
public static class PluginExtensions
{
    /// <summary>
    /// Gets the loaded plugin defined in the same assembly as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">A type from the target plugin assembly.</typeparam>
    /// <param name="plugins">The plugin service to search.</param>
    /// <returns>The plugin whose implementation assembly contains <typeparamref name="T" />.</returns>
    /// <exception cref="InvalidOperationException">No loaded plugin is defined in the assembly containing <typeparamref name="T" />.</exception>
    public static IPlugin GetPluginFromType<T>(this IPluginService plugins)
    {
        return plugins.GetPluginFromType(typeof(T));
    }

    /// <summary>
    /// Gets the loaded plugin defined in the same assembly as a runtime type.
    /// </summary>
    /// <param name="plugins">The plugin service to search.</param>
    /// <param name="type">A type from the target plugin assembly.</param>
    /// <returns>The plugin whose implementation assembly contains <paramref name="type" />.</returns>
    /// <exception cref="InvalidOperationException">No loaded plugin is defined in the assembly containing <paramref name="type" />.</exception>
    public static IPlugin GetPluginFromType(this IPluginService plugins, Type type)
    {
        if (!plugins.TryGetPluginFromType(type, out var plugin))
            throw new InvalidOperationException($"Plugin for packet {type.Name} not found.");

        return plugin;
    }

    /// <summary>
    /// Attempts to find the loaded plugin defined in the same assembly as a runtime type.
    /// </summary>
    /// <param name="plugins">The plugin service to search.</param>
    /// <param name="type">A type from the target plugin assembly.</param>
    /// <param name="plugin">When this method returns <see langword="true" />, the first matching loaded plugin; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when a matching plugin is loaded; otherwise, <see langword="false" />.</returns>
    public static bool TryGetPluginFromType(this IPluginService plugins, Type type, [MaybeNullWhen(false)] out IPlugin plugin)
    {
        plugin = plugins.All.FirstOrDefault(plugin => plugin.GetType().Assembly == type.Assembly);
        return plugin is not null;
    }
}
