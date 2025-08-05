using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Extensions;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Platform;

public class PlatformService(ILogger<PlatformService> logger, IHostApplicationLifetime hostApplicationLifetime, IPluginService plugins, ICommandService commands, Plugin plugin) : IPluginCommonService
{
    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != plugin)
            return;

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

        if (context.Source is IPlayer player)
        {
            await player.SendChatMessageAsync($"Loaded plugins: {names}", cancellationToken);
        }
        else
        {
            logger.LogInformation("Loaded plugins: {List}", names);
        }

        return 0;
    }

    private async ValueTask<int> UnloadContainerAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var containerName = context.GetArgument<string>("container");
        var container = plugins.Containers.FirstOrDefault(container => container.Contains(containerName, StringComparison.OrdinalIgnoreCase));

        if (container is null)
        {
            await SendAnswerAsync($"Container '{containerName}' not found");
            return 1;
        }

        logger.LogWarning("Unloading '{ContainerName}' container requested by {Source}", containerName, context.Source);

        await plugins.UnloadContainerAsync(container, cancellationToken);
        await SendAnswerAsync($"Container '{container}' unloaded successfully");

        return 0;

        async ValueTask SendAnswerAsync(string message)
        {
            if (context.Source is IPlayer player)
            {
                await player.SendChatMessageAsync(message, cancellationToken);
            }
            else
            {
                logger.LogError("{Message}", message);
            }
        }
    }
}
