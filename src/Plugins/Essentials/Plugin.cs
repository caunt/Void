using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Essentials.Moderation;
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
    }
}