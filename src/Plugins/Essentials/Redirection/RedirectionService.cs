using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Servers;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Redirection;

public class RedirectionService(ILogger<RedirectionService> logger, Plugin plugin, IServerService servers, ICommandService commands, ILinkService links) : IPluginCommonService
{
    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != plugin)
            return;

        commands.Register(builder => builder
            .Literal("server")
            .Then(builder => builder
                .Argument("server", Arguments.GreedyString())
                .Executes(ChangeServerAsync))
            .Executes(ChangeServerAsync));
    }

    public async ValueTask<int> ChangeServerAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Source is not IPlayer player)
        {
            logger.LogInformation("This command can be executed only by a player");
            return 1;
        }

        var previousServer = player.Server;
        var targetServer = context.TryGetArgument<string>("server", out var serverText) switch
        {
            true when servers.All.FirstOrDefault(server => server.Name.Equals(serverText, StringComparison.CurrentCultureIgnoreCase)) is { } found => found,
            _ => servers.All.Except([previousServer]).ElementAt(Random.Shared.Next(servers.All.Count() - 1))
        };

        if (targetServer is null)
            return 1;

        await links.ConnectAsync(player, targetServer, cancellationToken);

        var currentServer = player.Link?.Server ?? throw new InvalidOperationException("Player is not linked to any server after redirection.");

        player.Context.Logger.LogInformation("Redirected from server {PreviousServer} to server {CurrentServer}", previousServer, currentServer);

        return 0;
    }
}
