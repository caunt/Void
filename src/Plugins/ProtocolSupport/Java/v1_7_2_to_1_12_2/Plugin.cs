using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Crypto;
using Void.Proxy.Plugins.Common.Network.Channels.Services;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Channels;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Encryption;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Lifecycle;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2;

public class Plugin(IDependencyService dependencies) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_7_2, ProtocolVersion.MINECRAFT_1_12_2);

    public string Name => nameof(v1_7_2_to_1_12_2);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        Registry.Fill();

        dependencies.Register(services =>
        {
            services.AddSingleton<ChannelService>();
            services.AddSingleton<RegistryService>();
            services.AddSingleton<CompressionService>();
            services.AddSingleton<EncryptionService>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<TransformationService>();

            services.AddSingleton<CommandService>();
            services.AddSingleton<LifecycleService>();

            services.AddScoped<IChannelBuilderService, SimpleMinecraftChannelBuilderService>();
            services.AddScoped<ITokenHolder, SimpleTokenHolder>();
        });
    }
}
