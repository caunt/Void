using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Network;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.ForwardingSupport.Velocity.Services;

namespace Void.Proxy.Plugins.ForwardingSupport.Velocity;

public class Plugin(IDependencyService dependencies, IConfigurationService configs, IConsoleService console) : IProtocolPlugin
{
    public readonly Option<string> ForwardingModernKeyOption = new("--forwarding-modern-key")
    {
        Description = "Sets the secret key for modern forwarding"
    };

    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range();

    public string Name => nameof(Velocity);

    [Subscribe]
    public async ValueTask OnPluginLoading(PluginLoadingEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != this)
            return;

        var settings = await configs.GetAsync<Settings>(cancellationToken);

        if (string.IsNullOrWhiteSpace(settings.Secret))
            settings.Secret = Guid.NewGuid().ToString("N");

        dependencies.Register(services =>
        {
            services.AddSingleton(settings);
            services.AddScoped<ForwardingService>();
        });

        console.EnsureOptionDiscovered(ForwardingModernKeyOption);
    }
}
