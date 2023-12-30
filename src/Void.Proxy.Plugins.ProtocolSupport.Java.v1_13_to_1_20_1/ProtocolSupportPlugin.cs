using Serilog;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1;

public class ProtocolSupportPlugin : IPlugin
{
    public required ILogger Logger { get; init; }
    public string Name => nameof(ProtocolSupportPlugin);

    public readonly ProtocolVersion OldestVersion = ProtocolVersion.MINECRAFT_1_13;
    public readonly ProtocolVersion NewestVersion = ProtocolVersion.MINECRAFT_1_20;

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [Subscribe]
    public void OnProxyStart(ProxyStart @event)
    {
    }

    [Subscribe]
    public void OnProxyStop(ProxyStop @event)
    {
    }

    [Subscribe]
    public void OnSearchProtocolCodec(SearchClientProtocolCodec @event)
    {
    }
}
