using System.CommandLine;
using System.CommandLine.Invocation;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Servers;

public class ServerService(ISettings settings, ILinkService links, InvocationContext context) : IServerService
{
    private static readonly Option<bool> _ignoreFileServersOption = new("--ignore-file-servers", description: "Ignore servers specified in configuration files");

    public static void RegisterOptions(Command command)
    {
        command.AddOption(_ignoreFileServersOption);
    }

    public IEnumerable<IServer> All
        => (context.ParseResult.GetValueForOption(_ignoreFileServersOption) ? [] : settings.Servers)
            .Concat(links.All.Select(link => link.Server));
}
