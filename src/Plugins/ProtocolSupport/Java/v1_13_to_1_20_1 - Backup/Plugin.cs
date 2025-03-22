using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1;

public class Plugin(IEventService events) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_13, ProtocolVersion.MINECRAFT_1_20);

    public string Name => nameof(Plugin);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;

        Mappings.Fill();

        // events.RegisterListener<ChannelService>();
        // events.RegisterListener<ChannelCoordinatorService>();
        // events.RegisterListener<RegistryService>(this);
        // events.RegisterListener<CompressionService>();
        // events.RegisterListener<EncryptionService>();
    }
}