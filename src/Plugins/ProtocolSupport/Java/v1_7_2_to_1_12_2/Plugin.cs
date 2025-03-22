using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Channels;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Encryption;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Lifecycle;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2;

public class Plugin(IEventService events) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_7_2, ProtocolVersion.MINECRAFT_1_12_2);

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
        events.RegisterListener<LifecycleService>();
    }
}