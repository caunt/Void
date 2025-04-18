using System.Diagnostics.CodeAnalysis;

namespace Void.Proxy.Api.Plugins.Extensions;

public static class PluginExtensions
{
    public static bool TryGetPlugin(this IPluginService plugins, Type type, [MaybeNullWhen(false)] out IPlugin plugin)
    {
        plugin = plugins.All.FirstOrDefault(plugin => plugin.GetType().Assembly == type.Assembly);
        return plugin is not null;
    }
}
