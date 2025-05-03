using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.ExamplePlugin.Services;

namespace Void.Proxy.Plugins.ExamplePlugin;

// Implementing IPlugin makes class an entry point to your plugin
// Constructor arguments are used to inject many API services implemented by proxy and other plugins
// See ../Services/ directory for more examples
public class ExamplePlugin(ILogger logger, IDependencyService dependencies, ICommandService commands) : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        // This event is fired when any plugin is being loaded

        // Skip all other plugins load events except ours
        if (@event.Plugin != this)
            return;

        dependencies.Register(services =>
        {
            // Your services will be exposed to other plugins
            services.AddSingleton<ChatService>();

            // Scoped services are instantiated per-player
            services.AddScoped<InventoryService>();
        });

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
            // Commands might be triggered by console, plugins, or anything
            if (context.Source is not IPlayer player)
            {
                logger.LogInformation("This command can be executed only by player");
                return 1;
            }

            // You can resolve services manually from player context
            var inventory = player.Context.Services.GetRequiredService<InventoryService>();
            var result = await inventory.ChangeSlotAsync(context, cancellationToken);

            return result;
        }
    }

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event)
    {
        logger.LogInformation("Received ProxyStarting event");
    }

    [Subscribe]
    public void OnProxyStopping(ProxyStoppingEvent @event)
    {
        logger.LogInformation("Received ProxyStopping event");
    }
}
