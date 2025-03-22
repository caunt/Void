using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.IO.Bundles;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Common.Network.Protocol.Bundles;

public abstract class AbstractBundleService : IPluginService
{
    [Subscribe]
    public void OnPlayerConnecting(PlayerConnectingEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Services.HasService<IBundleService>())
            @event.Services.AddSingleton<IBundleService, BundleService>();
    }

    [Subscribe(PostOrder.First)]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (!IsBundlePacket(@event.Message))
            return;

        var bundles = @event.Link.Player.Context.Services.GetRequiredService<IBundleService>();
        bundles.ToggleBundle();
    }

    protected abstract bool IsBundlePacket(IMinecraftMessage message);
}