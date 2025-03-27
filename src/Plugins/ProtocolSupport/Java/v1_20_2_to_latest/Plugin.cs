using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Channels;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Encryption;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Lifecycle;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest;

public class Plugin(IEventService events) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Plugin);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;

        Registry.Fill();

        events.RegisterListener<RegistryService>(this);
        events.RegisterListener<ChannelService>();
        events.RegisterListener<CompressionService>();
        events.RegisterListener<EncryptionService>();
        events.RegisterListener<AuthenticationService>();

        events.RegisterListener<CommandService>();
        events.RegisterListener<BundleService>();
        events.RegisterListener<LifecycleService>();
    }
}