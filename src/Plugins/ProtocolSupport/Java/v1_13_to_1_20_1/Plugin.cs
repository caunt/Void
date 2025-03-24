using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Channels;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Encryption;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Lifecycle;
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

        Registry.Fill();

        events.RegisterListener<ChannelService>();
        events.RegisterListener<RegistryService>(this);
        events.RegisterListener<CompressionService>();
        events.RegisterListener<EncryptionService>();
        events.RegisterListener<AuthenticationService>();

        events.RegisterListener<CommandService>();
        events.RegisterListener<BundleService>();
        events.RegisterListener<LifecycleService>();
    }
}