using Void.Common.Events;
using Void.Common.Network;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

public class ChatService(ILinkService links) : IEventListener
{
    [Subscribe]
    public async ValueTask OnPhaseChanged(PhaseChangedEvent @event, CancellationToken cancellationToken)
    {
        if (@event is not { Phase: Phase.Play, Side: Side.Client })
            return;

        if (!links.TryGetLink(@event.Player, out var link))
            return;

        await @event.Player.SendChatMessageAsync($"Welcome to the {link.Server} server!", cancellationToken);
    }
}
