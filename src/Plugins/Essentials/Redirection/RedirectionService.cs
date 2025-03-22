using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Commands;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Redirection;

public class RedirectionService(IServerService servers, ILogger<RedirectionService> logger) : IPluginService
{
    private readonly ConcurrentDictionary<IPlayer, IServer> _connecting = [];

    [Subscribe]
    public void OnChatCommand(ChatCommandEvent @event)
    {
        var parts = @event.Command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is 0)
            return;

        switch (parts[0].ToLower())
        {
            case "server":
                @event.Result = true;

                // find selected IServer or choose random from registered servers
                var server = parts.Length switch
                {
                    > 1 when servers.RegisteredServers.FirstOrDefault(server => server.Name.Equals(parts[1], StringComparison.CurrentCultureIgnoreCase)) is { } found => found,
                    _ => servers.RegisteredServers.Except([@event.Link.Server]).ElementAt(Random.Shared.Next(servers.RegisteredServers.Count - 1))
                };

                if (_connecting.ContainsKey(@event.Link.Player))
                    _connecting[@event.Link.Player] = server;
                else if (_connecting.TryAdd(@event.Link.Player, server))
                    @event.Link.ServerChannel.Close();

                break;
        }
    }

    [Subscribe]
    public void OnPlayerSearchServer(PlayerSearchServerEvent @event)
    {
        if (!_connecting.TryRemove(@event.Player, out var server))
            return;

        @event.Result = server;
        logger.LogInformation("Player {Player} connected to server {Server}", @event.Player, server);
    }
}
