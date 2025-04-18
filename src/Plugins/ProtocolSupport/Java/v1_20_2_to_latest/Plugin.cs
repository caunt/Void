using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Channels;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Encryption;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Lifecycle;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest;

public class Plugin(IDependencyService dependencies) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(v1_20_2_to_latest);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        Registry.Fill();

        dependencies.CreateInstance<RegistryService>();
        dependencies.CreateInstance<ChannelService>();
        dependencies.CreateInstance<CompressionService>();
        dependencies.CreateInstance<EncryptionService>();
        dependencies.CreateInstance<AuthenticationService>();
        dependencies.CreateInstance<TransformationService>();

        dependencies.CreateInstance<CommandService>();
        dependencies.CreateInstance<BundleService>();
        dependencies.CreateInstance<LifecycleService>();
    }
}
