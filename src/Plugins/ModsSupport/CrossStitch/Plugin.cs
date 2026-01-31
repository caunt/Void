using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.ModsSupport.CrossStitch.Services;

namespace Void.Proxy.Plugins.ModsSupport.CrossStitch;

public class Plugin(IDependencyService dependencies) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range();

    public string Name => nameof(CrossStitch);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        dependencies.Register(services => services.AddSingleton<CrossStitchService>());
    }
}
