using Void.Common.Events;
using Void.Common.Network;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

public class ChatService(ILinkService links, IConfigurationService configs) : IEventListener
{
    [Subscribe]
    public async ValueTask OnPhaseChanged(PhaseChangedEvent @event, CancellationToken cancellationToken)
    {
        if (@event is not { Phase: Phase.Play, Side: Side.Client })
            return;

        if (!links.TryGetLink(@event.Player, out var link))
            return;

        var settings = await configs.GetAsync<ChatSettings>(cancellationToken);
        var message = settings.WelcomeMessage.Replace("%SERVER%", link.Server.ToString());

        await @event.Player.SendChatMessageAsync(message, cancellationToken);
    }
}

public class ChatSettings
{
    public string WelcomeMessage { get; set; } = "Welcome to the %SERVER% server!";
}
