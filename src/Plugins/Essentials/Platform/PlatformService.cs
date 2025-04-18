using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Extensions;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Platform;

public class PlatformService(ILogger<PlatformService> logger, IHostApplicationLifetime hostApplicationLifetime, IPluginService plugins, ICommandService commands) : IPluginCommonService
{
    [Subscribe]
    public void OnProxyStarted(ProxyStartedEvent _)
    {
        commands.Register(builder => builder
            .Literal("stop")
            .Executes(StopServer));

        commands.Register(builder => builder
            .Literal("plugins")
            .Executes(ListContainersAsync));

        commands.Register(builder => builder
            .Literal("unload")
            .Then(builder => builder
                .Argument("container", Arguments.String())
                .Executes(UnloadContainerAsync)));
    }

    private int StopServer(CommandContext context)
    {
        logger.LogInformation("Stopping proxy...");
        hostApplicationLifetime.StopApplication();
        return 0;
    }

    private async ValueTask<int> ListContainersAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var names = string.Join(", ", plugins.Containers);

        if (context.Source is IMinecraftPlayer player)
        {
            await player.SendChatMessageAsync("Loaded plugins: " + names, cancellationToken);
        }
        else
        {
            logger.LogInformation("Loaded plugins: {List}", names);
        }

        return 0;
    }

    private async ValueTask<int> UnloadContainerAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var container = context.GetArgument<string>("container");
        await plugins.UnloadContainerAsync(container, cancellationToken);
        return 0;
    }
}
