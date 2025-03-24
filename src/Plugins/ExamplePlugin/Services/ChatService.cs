using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Minecraft;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players.Extensions;

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
