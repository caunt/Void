using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Servers;

public class ServerService(ILogger<ServerService> logger, ISettings settings, IConsoleService console) : IServerService, IEventListener
{
    /// <summary>
    /// The default Minecraft server port.
    /// </summary>
    private const int DefaultPort = 25565;

    private static readonly Option<bool> _ignoreFileServersOption = new("--ignore-file-servers")
    {
        Description = "Ignore servers specified in configuration files"
    };

    private static readonly Option<string[]> _serversOption = new("--server")
    {
        Description = "Registers an additional server in format <host>:<port> or <host> (port defaults to 25565)"
    };

    public IEnumerable<IServer> All => GetArgumentsServers().Concat(console.GetOptionValue(_ignoreFileServersOption) ? [] : settings.Servers);

    [Subscribe]
    public void OnProxyStarted(ProxyStartingEvent @event)
    {
        var servers = All.ToList();

        if (!servers.Any())
        {
            logger.LogWarning("No servers are registered");
            return;
        }

        logger.LogInformation("Registered servers:");

        foreach (var server in servers)
            logger.LogInformation(" - {Server} ({Address}:{Port})", server.Name, server.Host, server.Port);
    }

    public bool TryGetByName(string name, [MaybeNullWhen(false)] out IServer server)
    {
        server = GetByName(name);
        return server is not null;
    }

    public IServer? GetByName(string name)
    {
        return All.FirstOrDefault(server => server.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private IEnumerable<Server> GetArgumentsServers()
    {
        var servers = console.GetOptionValue(_serversOption);

        if (servers == null)
            yield break;

        var index = 1;

        foreach (var argument in servers)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                logger.LogWarning("Invalid server {Server}", argument);
                continue;
            }

            // Try to parse with Uri first (this handles the host:port format)
            if (!Uri.TryCreate($"tcp://{argument}", UriKind.Absolute, out var uri))
            {
                logger.LogWarning("Invalid server {Server}", argument);
                continue;
            }

            var port = uri.Port;
            var host = uri.Host;

            // Uri.Port returns -1 when no port is specified in the URI
            if (port == -1)
                port = DefaultPort;

            if (port is < 1 or > 65535 || string.IsNullOrWhiteSpace(host))
            {
                logger.LogWarning("Invalid server {Server}", argument);
                continue;
            }

            yield return new Server($"args-server-{index++}", host, port);
        }
    }
}
