using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Plugins.Common.Network.Bundles;

namespace Void.Proxy.Plugins.Common.Services.Bundles;

public abstract class AbstractBundleService : IPluginCommonService
{
    [Subscribe(PostOrder.First)]
    public void OnMessageSent(MessageSentEvent @event)
    {
        if (!IsBundlePacket(@event.Message))
            return;

        var bundles = @event.Player.Context.Services.GetRequiredService<IBundleService>();
        bundles.ToggleBundle();
    }

    protected abstract bool IsBundlePacket(INetworkMessage message);
}
