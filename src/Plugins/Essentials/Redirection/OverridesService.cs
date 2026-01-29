using System.Collections.Concurrent;
using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Servers;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Redirection;

public class OverridesService(ILogger<OverridesService> logger, IServerService servers, ILinkService links, IConsoleService console) : IPluginCommonService
{
    public readonly Option<string[]> OverridesOption = new("--override", "-o")
    {
        Description = "Register an additional server override to redirect players based on hostname they are connecting with.\nExample:\n--ignore-file-servers\n--server 127.0.0.1:25565\n--override vanilla.example.org=args-server-1\nIf you configured server in file:\n--override vanilla.example.org=lobby"
    };

    private readonly ConcurrentDictionary<ILink, IServer> _queuedRedirections = new();

    public static void ValidateOverride(OptionResult result)
    {
        foreach (var option in result.GetValueOrDefault<string[]>())
        {
            var parts = option.Split('=');

            if (parts.Length is 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]))
                continue;

            result.AddError($"Override \"{option}\" must be in the format <hostname>=<server-name>.");
            return;
        }
    }

    [Subscribe]
    public async ValueTask OnHandshakeCompleted(HandshakeCompletedEvent @event, CancellationToken cancellationToken)
    {
        var overrides = console.GetOptionValue(OverridesOption);

        if (overrides is null)
            return;

        foreach (var value in overrides)
        {
            var parts = value.Split("=");

            if (parts.Length is not 2)
                continue;

            var overrideName = parts[0];
            var serverName = parts[1];

            if (!string.Equals(@event.ServerAddress, overrideName, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!servers.TryGetByName(serverName, out var server))
            {
                logger.LogWarning("Failed to find server {Server} from redirection override name {Name} for player {Player}.", serverName, overrideName, @event.Player);
                continue;
            }

            if (!_queuedRedirections.TryAdd(@event.Link, server))
                throw new InvalidOperationException("Failed to queue overriden redirection, player already has a queued redirection.");

            logger.LogTrace("Queued overriden redirection of player {Player} to server {Server}.", @event.Player, server.Name);
            break;
        }
    }

    [Subscribe(PostOrder.First)]
    public async ValueTask OnPlayerJoinedServer(PlayerJoinedServerEvent @event, CancellationToken cancellationToken)
    {
        if (!_queuedRedirections.TryRemove(@event.Link, out var server))
            return;

        @event.IsRedirecting = true;

        var previousServer = @event.Link.Server;
        var connectionTask = Task.Run(async () =>
        {
            try
            {
                var result = await links.ConnectAsync(@event.Player, server, cancellationToken);

                if (result is ConnectionResult.NotConnected)
                {
                    logger.LogWarning("Failed to redirect player {Player} to server {Server}. Connecting player back to {PreviousServer}.", @event.Player, server, previousServer);
                    result = await links.ConnectAsync(@event.Player, previousServer, cancellationToken);
                }

                if (result is ConnectionResult.NotConnected)
                {
                    logger.LogError("Failed to reconnect player {Player} to previous server {PreviousServer} after failed redirection to {Server}.", @event.Player, previousServer, server);
                    await @event.Player.KickAsync("Failed to connect to any server after trying overrides.", cancellationToken);
                }

                logger.LogTrace("Redirected player {Player} to server {Server} override successfully.", @event.Player, server);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Exception occurred in override redirecting player {Player} to server {Server}.", @event.Player, server);
            }
        }, cancellationToken);

        // TODO: Consider awaiting connection task
    }
}
