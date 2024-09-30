using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Services;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest;

public class Plugin(IEventService events) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Plugin);

    [Subscribe]
    public ValueTask OnProxyStarting(ProxyStartingEvent @event)
    {
        return ValueTask.CompletedTask;
    }

    [Subscribe]
    public ValueTask OnProxyStopping(ProxyStoppingEvent @event)
    {
        return ValueTask.CompletedTask;
    }

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;

        Mappings.Fill();

        events.RegisterListeners<RegistryService>(this);
        events.RegisterListeners<ChannelService>();
        events.RegisterListeners<CompressionService>();
        events.RegisterListeners<EncryptionService>();
        events.RegisterListeners<ChannelCoordinatorService>();
    }
}