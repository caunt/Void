using Void.Common.Events;
using Void.Common.Network;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

public class ChatService(ILinkService links, IConfigurationService configs) : IEventListener
{
    private ChatSettings? _settings;

    [Subscribe]
    public async ValueTask OnProxyStarting(ProxyStartingEvent @event, CancellationToken cancellationToken)
    {
        // Configurations can be loaded once, and any changes made to them will be automatically saved to disk. 
        // Vice versa, any changes made to the configuration file on disk will be automatically loaded into the instance.
        // You do not need to worry about saving or loading the configuration manually.
        _settings = await configs.GetAsync<ChatSettings>(cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnPhaseChanged(PhaseChangedEvent @event, CancellationToken cancellationToken)
    {
        // Send message only when the connection is in the Play phase on the client side
        if (@event is not { Phase: Phase.Play, Side: Side.Client })
            return;

        // Only if player has active link to the server
        if (!links.TryGetLink(@event.Player, out var link))
            return;

        // If settings are not loaded, do nothing. Practically impossible, but just in case.
        if (_settings is null)
            return;

        // Replace placeholder in the message with the server name
        var message = _settings.WelcomeMessage.Replace("%SERVER%", link.Server.Name);

        await @event.Player.SendChatMessageAsync(message, cancellationToken);
    }
}

public class ChatSettings
{
    public string WelcomeMessage { get; set; } = $"Welcome to the %SERVER% server!";
}
