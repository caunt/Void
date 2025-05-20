using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Extensions;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

// Example Chat configuration class.
public class ChatSettings
{
    public string WelcomeMessage { get; set; } = $"Welcome to the %SERVER% server!";
}

// Here you can use DI to inject any service API you want to use.
public class ChatService(ILogger<ChatService> logger, IConfigurationService configs, ICommandService commands) : IEventListener
{
    // Save settings instance locally, it is safe and will be updated in place if changes are made on disk.
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
        // Send message only when the connection is in the Play phase on the client side.
        if (@event is not { Phase: Phase.Play, Side: Side.Client })
            return;

        // Link is a connection between player and server.
        // If player is being redirected to another server, another link will be created.

        // Only if player has active link to the server.
        if (!@event.Player.TryGetLink(out var link))
            return;

        // If settings are not loaded, do nothing. Practically impossible, but just in case.
        if (_settings is null)
            return;

        // Replace placeholder in the message with the server name
        var message = _settings.WelcomeMessage.Replace("%SERVER%", link.Server.Name);

        await @event.Player.SendChatMessageAsync(message, cancellationToken);
    }

    // Registering commands and interaction with scoped services.
    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        // This event is fired when any plugin is being loaded

        // Skip all other plugins load events except ours
        if (@event.Plugin != this)
            return;

        // Register your commands in brigadier-like way
        // https://github.com/Mojang/brigadier/
        commands.Register(builder => builder
            .Literal("slot")
            .Executes(SlotCommandAsync)
            .Then(builder => builder
                .Argument("index", Arguments.Integer())
                .Executes(SlotCommandAsync)));

        async ValueTask<int> SlotCommandAsync(CommandContext context, CancellationToken cancellationToken)
        {
            // Commands might be triggered by console, plugins, players, or anything
            if (context.Source is not IPlayer player)
            {
                logger.LogInformation("This command can be executed only by player");
                return 1;
            }

            // You can resolve scoped services manually from player context
            var inventory = player.Context.Services.GetRequiredService<InventoryService>();
            var result = await inventory.ChangeSlotAsync(context, cancellationToken);

            return result;
        }
    }
}
