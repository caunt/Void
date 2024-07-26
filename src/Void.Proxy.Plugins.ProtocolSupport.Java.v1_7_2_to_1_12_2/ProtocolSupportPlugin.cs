using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2;

public class ProtocolSupportPlugin(ILogger<ProtocolSupportPlugin> logger) : IPlugin
{
    public readonly ProtocolVersion NewestVersion = ProtocolVersion.MINECRAFT_1_12_2;

    public readonly ProtocolVersion OldestVersion = ProtocolVersion.MINECRAFT_1_7_2;
    public string Name => nameof(ProtocolSupportPlugin);

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event)
    {
    }

    [Subscribe]
    public void OnProxyStopping(ProxyStoppingEvent @event)
    {
    }

    [Subscribe]
    public void OnSearchChannelBuilder(SearchChannelBuilderEvent @event)
    {
    }
}