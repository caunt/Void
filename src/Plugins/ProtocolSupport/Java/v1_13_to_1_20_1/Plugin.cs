using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Crypto;
using Void.Proxy.Plugins.Common.Network.Channels.Services;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Channels;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Encryption;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Lifecycle;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1;

public class Plugin(IDependencyService dependencies) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_13, ProtocolVersion.MINECRAFT_1_20);

    public string Name => nameof(v1_13_to_1_20_1);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        Registry.Fill();

        dependencies.Register(services =>
        {
            services.AddScoped<Common.Network.Bundles.IBundleService, Common.Network.Bundles.BundleService>();
            services.AddScoped<IChannelBuilderService, SimpleMinecraftChannelBuilderService>();
            services.AddScoped<ITokenHolder, SimpleTokenHolder>();
        });

        dependencies.CreateInstance<ChannelService>();
        dependencies.CreateInstance<RegistryService>();
        dependencies.CreateInstance<CompressionService>();
        dependencies.CreateInstance<EncryptionService>();
        dependencies.CreateInstance<AuthenticationService>();
        dependencies.CreateInstance<TransformationService>();

        dependencies.CreateInstance<CommandService>();
        dependencies.CreateInstance<BundleService>();
        dependencies.CreateInstance<LifecycleService>();
    }
}
