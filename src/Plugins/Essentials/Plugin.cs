using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.Essentials.Debugging;
using Void.Proxy.Plugins.Essentials.Moderation;
using Void.Proxy.Plugins.Essentials.Platform;
using Void.Proxy.Plugins.Essentials.Redirection;

namespace Void.Proxy.Plugins.Essentials;

public class Plugin(IDependencyService dependencies) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Essentials);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;

        dependencies.CreateInstance<RedirectionService>();
        dependencies.CreateInstance<ModerationService>();
        dependencies.CreateInstance<PlatformService>();
        dependencies.CreateInstance<TraceService>();
    }
}
