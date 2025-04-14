using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.Essentials.Debugging;
using Void.Proxy.Plugins.Essentials.Moderation;
using Void.Proxy.Plugins.Essentials.Platform;
using Void.Proxy.Plugins.Essentials.Redirection;

namespace Void.Proxy.Plugins.Essentials;

public class Plugin(IEventService events) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Plugin);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;

        events.RegisterListener<RedirectionService>();
        events.RegisterListener<ModerationService>();
        events.RegisterListener<PlatformService>();
        events.RegisterListener<TraceService>();
    }
}
