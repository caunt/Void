using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Servers;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Redirection;

public class RedirectionService(ILogger<RedirectionService> logger, Plugin plugin, IServerService servers, ICommandService commands) : IPluginCommonService
{
    private readonly ConcurrentDictionary<IPlayer, IServer> _connecting = [];

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != plugin)
            return;

        commands.Register(builder => builder
            .Literal("server")
            .Then(builder => builder
                .Argument("server", Arguments.GreedyString())
                .Executes(ChangeServer))
            .Executes(ChangeServer));
    }

    public int ChangeServer(CommandContext context)
    {
        if (context.Source is not IPlayer player)
        {
            logger.LogInformation("This command can be executed only by player");
            return 1;
        }

        var currentServer = player.GetServer();
        var nextServer = context.TryGetArgument<string>("server", out var serverText) switch
        {
            true when servers.RegisteredServers.FirstOrDefault(server => server.Name.Equals(serverText, StringComparison.CurrentCultureIgnoreCase)) is { } found => found,
            _ => servers.RegisteredServers.Except([currentServer]).ElementAt(Random.Shared.Next(servers.RegisteredServers.Count - 1))
        };

        if (nextServer is null)
            return 1;

        if (_connecting.ContainsKey(player))
        {
            _connecting[player] = nextServer;
            return 0;
        }

        if (_connecting.TryAdd(player, nextServer))
        {
            if (player.TryGetLink(out var link))
                link.ServerChannel.Close();

            return 0;
        }

        return 0;
    }

    [Subscribe]
    public void OnPlayerSearchServer(PlayerSearchServerEvent @event)
    {
        if (!_connecting.TryRemove(@event.Player, out var server))
            return;

        @event.Result = server;
    }
}
