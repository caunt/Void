using System.CommandLine;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Servers;

public class ServerService(ILogger<ServerService> logger, ISettings settings, IConsoleService console) : IServerService
{
    private static readonly Option<bool> _ignoreFileServersOption = new("--ignore-file-servers")
    {
        Description = "Ignore servers specified in configuration files"
    };

    private static readonly Option<string[]> _serversOption = new("--server")
    {
        Description = "Registers an additional server in format <host>:<port>"
    };

    public IEnumerable<IServer> All => GetArgumentsServers().Concat(console.GetOptionValue(_ignoreFileServersOption) ? [] : settings.Servers);

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

            if (!Uri.TryCreate($"tcp://{argument}", UriKind.Absolute, out var uri))
            {
                logger.LogWarning("Invalid server {Server}", argument);
                continue;
            }

            var port = uri.Port;
            var host = uri.Host;

            if (port is < 1 or > 65535 || string.IsNullOrWhiteSpace(host))
            {
                logger.LogWarning("Invalid server {Server}", argument);
                continue;
            }

            yield return new Server($"args-server-{index++}", host, port);
        }
    }
}
