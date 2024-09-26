using Microsoft.Extensions.Logging;
using MinecraftNotifier.Lib;
using MinecraftNotifier.Lib.Models;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2;

public class Plugin(ILogger<Plugin> logger) : IPlugin
{
    public readonly ProtocolVersion NewestVersion = ProtocolVersion.MINECRAFT_1_12_2;

    public readonly ProtocolVersion OldestVersion = ProtocolVersion.MINECRAFT_1_7_2;
    public string Name => nameof(Plugin);

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event, CancellationToken cancellationToken)
    {
        var type = typeof(MinecraftVersion);
        logger.LogDebug("TEST: {Value}", type.GetConstructors()[0].ToString());

        var instance = new MinecraftVersion();

        MinecraftVersion? declaration = null;
        declaration?.Start();

        logger.LogDebug("TEST: {Value}", declaration == null);

        var feed = new MinecraftRssFeed { RssChannel = new RssChannel { Description = "test" } };
        logger.LogDebug("TEST: {Value}", feed.RssChannel.Description);
        logger.LogDebug("TEST: {Value}", instance.ToString());

        var mapping = new PacketMapping(1, ProtocolVersion.MINECRAFT_1_10);
        logger.LogDebug("TEST: {Value}", mapping.ToString());
    }

    [Subscribe]
    public void OnProxyStopping(ProxyStoppingEvent @event, CancellationToken cancellationToken)
    {
    }

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != this)
            return;
    }

    [Subscribe]
    public void OnPluginUnload(PluginUnloadEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != this)
            return;
    }

    [Subscribe]
    public void OnSearchChannelBuilder(SearchChannelBuilderEvent @event, CancellationToken cancellationToken)
    {
    }
}