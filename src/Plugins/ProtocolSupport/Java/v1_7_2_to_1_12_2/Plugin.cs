using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2;

public class Plugin : IPlugin
{
    public readonly ProtocolVersion NewestVersion = ProtocolVersion.MINECRAFT_1_12_2;

    public readonly ProtocolVersion OldestVersion = ProtocolVersion.MINECRAFT_1_7_2;
    public string Name => nameof(Plugin);

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event, CancellationToken cancellationToken)
    {
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