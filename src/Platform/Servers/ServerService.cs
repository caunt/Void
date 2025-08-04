using System.CommandLine;
using System.CommandLine.Invocation;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Servers;

public class ServerService(ILogger<ServerService> logger, ISettings settings, InvocationContext context) : IServerService
{
    private static readonly Option<bool> _ignoreFileServersOption = new("--ignore-file-servers", description: "Ignore servers specified in configuration files");
    private static readonly Option<string[]> _serversOption = new("--server", description: "Registers an additional server in format <host>:<port>");

    public static void RegisterOptions(Command command)
    {
        command.AddOption(_ignoreFileServersOption);
        command.AddOption(_serversOption);
    }

    public IEnumerable<IServer> All => GetArgumentsServers().Concat(context.ParseResult.GetValueForOption(_ignoreFileServersOption) ? [] : settings.Servers);

    private IEnumerable<Server> GetArgumentsServers()
    {
        var servers = context.ParseResult.GetValueForOption(_serversOption);

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
