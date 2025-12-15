using System.Diagnostics.CodeAnalysis;

namespace Void.Proxy.Api.Plugins.Extensions;

public static class PluginExtensions
{
    public static IPlugin GetPluginFromType<T>(this IPluginService plugins)
    {
        return plugins.GetPluginFromType(typeof(T));
    }

    public static IPlugin GetPluginFromType(this IPluginService plugins, Type type)
    {
        if (!plugins.TryGetPluginFromType(type, out var plugin))
            throw new InvalidOperationException($"Plugin for packet {type.Name} not found.");

        return plugin;
    }

    public static bool TryGetPluginFromType(this IPluginService plugins, Type type, [MaybeNullWhen(false)] out IPlugin plugin)
    {
        plugin = plugins.All.FirstOrDefault(plugin => plugin.GetType().Assembly == type.Assembly);
        return plugin is not null;
    }
}
