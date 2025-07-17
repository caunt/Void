using System.CommandLine;
using System.CommandLine.Invocation;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Servers;

public class ServerService(ISettings settings, ILinkService links, InvocationContext context) : IServerService
{
    private static readonly Option<bool> _ignoreFileServersOption = new("--ignore-file-servers", description: "Ignore servers specified in configuration files");
    private static readonly Option<string[]> _serversOption = new("--server", description: "Registers an additional server in format <host>:<port>");

    public static void RegisterOptions(Command command)
    {
        command.AddOption(_ignoreFileServersOption);
        command.AddOption(_serversOption);
    }

    public IEnumerable<IServer> All
        => (context.ParseResult.GetValueForOption(_ignoreFileServersOption) ? [] : settings.Servers)
            .Concat(GetArgumentsServers())
            .Concat(links.All.Select(link => link.Server));

    private IEnumerable<Server> GetArgumentsServers()
    {
        var servers = context.ParseResult.GetValueForOption(_serversOption);

        if (servers == null)
            yield break;

        var index = 1;

        foreach (var argument in servers)
        {
            if (string.IsNullOrWhiteSpace(argument))
                continue;

            var parts = argument.Split(':', 2);

            if (parts.Length != 2)
                continue;

            if (!int.TryParse(parts[1], out var port))
                continue;

            yield return new Server($"args-server-{index++}", parts[0], port);
        }
    }
}
